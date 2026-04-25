using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service_Abstraction.IAdminService;
using LEX.Shared.DTOs;
using LEX.Shared.DTOs.Admin;
using LEX.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<ServiceResponse<List<PendingLawyerDto>>> GetPendingLawyersAsync()
        {
            var repo = _unitOfWork.GetRepository<Lawyer, int>();

            var lawyers = (await repo.GetAsync(
                l => l.Status == Status.Waiting,
                null,
                null,
                null,
                l => l.Documents
            )).ToList();

            var result = lawyers.Select(l => new PendingLawyerDto
            {
                Id = l.Id,
                Name = $"{l.Fname} {l.Lname}",
                Email = l.Email,
                Phone = l.Phone,
                ProfileImage = l.Porfile_Image,

                About = l.About,
                Education = l.Education,
                YearsOfExperience = DateTime.Now.Year - l.Start_Practice.Year,

                Documents = l.Documents.Select(d => new DocumentDto
                {
                    Id = d.Id,
                    Url = d.Document_URL,
                    Type = d.Document_Type.ToString()
                }).ToList()
            }).ToList();

            return ServiceResponse<List<PendingLawyerDto>>.SuccessResult(result);
        }


        public async Task<ServiceResponse<string>> AcceptLawyerAsync(int lawyerId)
        {
            var repo = _unitOfWork.GetRepository<Lawyer, int>();

            var lawyer = await repo.GetByIdAsync(lawyerId);

            if (lawyer == null)
                return ServiceResponse<string>.Fail("Lawyer not found");

            if (lawyer.Status != Status.Waiting)
                return ServiceResponse<string>.Fail("Lawyer already processed");

            lawyer.Status = Status.Active;
            lawyer.RejectionReason = null;

            repo.Update(lawyer);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<string>.SuccessResult("Lawyer accepted");
        }


        public async Task<ServiceResponse<string>> RejectLawyerAsync(int lawyerId, string reason)
        {
            var repo = _unitOfWork.GetRepository<Lawyer, int>();

            var lawyer = await repo.GetByIdAsync(lawyerId);

            if (lawyer == null)
                return ServiceResponse<string>.Fail("Lawyer not found");

            if (lawyer.Status != Status.Waiting)
                return ServiceResponse<string>.Fail("Lawyer already processed");

            if (string.IsNullOrWhiteSpace(reason))
                return ServiceResponse<string>.Fail("Rejection reason required");

            lawyer.Status = Status.Blocked;
            lawyer.RejectionReason = reason;

            repo.Update(lawyer);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<string>.SuccessResult("Lawyer rejected");
        }
    }
}
