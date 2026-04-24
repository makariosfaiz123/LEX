using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service_Abstraction;
using LEX.Shared.DTOs;
using LEX.Shared.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public DocumentService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<ServiceResponse<string>> DeleteDocumentAsync(int documentId, int userId)
        {
            var repo = _unitOfWork.GetRepository<Documents, int>();

            var doc = await repo.GetByIdAsync(documentId);

            if (doc == null)
                return ServiceResponse<string>.Fail("Document not found");

            if (doc.Lawyer_Id != userId)
                return ServiceResponse<string>.Fail("Unauthorized");

            repo.Delete(doc);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<string>.SuccessResult("Document deleted");
        }

        public async Task<ServiceResponse<string>> UpdateDocumentAsync(
                int documentId,
                int userId,
                Stream? fileStream,
                string? fileName,
                DocumentType? type
            )
        {
            var repo = _unitOfWork.GetRepository<Documents, int>();

            var doc = await repo.GetByIdAsync(documentId);

            if (doc == null)
                return ServiceResponse<string>.Fail("Document not found");

            if (doc.Lawyer_Id != userId)
                return ServiceResponse<string>.Fail("Unauthorized");

            if (fileStream == null && type == null)
                return ServiceResponse<string>.Fail("Nothing to update");

            // Replace File
            if (fileStream != null && fileName != null)
            {
                var ext = Path.GetExtension(fileName).ToLower();

                if (ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                    return ServiceResponse<string>.Fail("Images not allowed");

                var url = await _fileService.UploadFileAsync(fileStream, fileName);

                doc.Document_URL = url;
            }

            // Update Type
            if (type != null)
            {
                doc.Document_Type = type.Value;
            }

            repo.Update(doc);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<string>.SuccessResult("Document updated");
        }


        public async Task<ServiceResponse<List<DocumentDto>>> GetMyDocumentsAsync(int userId)
        {
            var repo = _unitOfWork.GetRepository<Documents, int>();

            var docs = await repo.GetAsync(d => d.Lawyer_Id == userId);

            var result = docs.Select(d => new DocumentDto
            {
                Id = d.Id,
                Url = d.Document_URL,
                Type = d.Document_Type.ToString()
            }).ToList();

            return ServiceResponse<List<DocumentDto>>.SuccessResult(result);
        }



        public async Task<ServiceResponse<string>> ResubmitDocumentsAsync(
                int userId,
                List<(Stream Stream, string FileName)> files,
                List<DocumentType> types
            )
        {
            var userRepo = _unitOfWork.GetRepository<Users, int>();
            var docRepo = _unitOfWork.GetRepository<Documents, int>();

            var user = await userRepo.GetByIdAsync(userId);

            // validation
            if (user == null || user.RoleS != RoleS.Lawyer)
                return ServiceResponse<string>.Fail("User not found");

            if (user.Status != Status.Blocked)
                return ServiceResponse<string>.Fail("You can only resubmit after rejection");

            if (files == null || files.Count == 0)
                return ServiceResponse<string>.Fail("No files uploaded");

            if (types == null || types.Count != files.Count)
                return ServiceResponse<string>.Fail("Each file must have a type");

            // Required docs
            var requiredTypes = new[]
            {
                DocumentType.Passport,
                DocumentType.SyndicateCard,
                DocumentType.LawyerLicense
            };

            var uploadedTypes = types.Distinct().ToList();

            foreach (var required in requiredTypes)
            {
                if (!uploadedTypes.Contains(required))
                    return ServiceResponse<string>.Fail($"Missing required document: {required}");
            }

            // delete old
            var oldDocs = (await docRepo.GetAsync(d => d.Lawyer_Id == userId)).ToList();

            foreach (var doc in oldDocs)
                docRepo.Delete(doc);

            //upload new
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var type = types[i];

                var ext = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(ext))
                    return ServiceResponse<string>.Fail("Invalid file type");

                if (file.Stream.Length > 5 * 1024 * 1024)
                    return ServiceResponse<string>.Fail("File too large (max 5MB)");

                var url = await _fileService.UploadFileAsync(
                    file.Stream,
                    file.FileName
                );

                await docRepo.AddAsync(new Documents
                {
                    Lawyer_Id = userId,
                    Document_URL = url,
                    Document_Type = type
                });
            }

            // update status
            user.Status = Status.Waiting;

            if (user is Lawyer lawyer)
                lawyer.RejectionReason = null;

            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<string>.SuccessResult("Documents resubmitted successfully");
        }


    }
}
