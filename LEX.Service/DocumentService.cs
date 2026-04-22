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
    }
}
