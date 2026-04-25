using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service.Helpers;
using LEX.Service_Abstraction;
using LEX.Service_Abstraction.CaseRequests;
using LEX.Shared.DTOs.CaseRequest;
using LEX.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service.CaseRequests
{
    public class CaseRequestService : ICaseRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public CaseRequestService(IUnitOfWork unitOfWork,INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<ServiceResponse<string>> CreateCaseAsync(CreateCaseRequestDto dto, int userId)
        {
            var caseRepo = _unitOfWork.GetRepository<CaseRequest, int>();
            var lawyerRepo = _unitOfWork.GetRepository<Lawyer, int>();

            //Validate CaseType
            if (!EnumHelper.TryParseEnum<CaseType>(dto.CaseType, out var caseType))
                return ServiceResponse<string>.Fail("Invalid CaseType");

            if (string.IsNullOrWhiteSpace(dto.Description))
                return ServiceResponse<string>.Fail("Description is required");

            dto.Description = dto.Description.Trim();

            if (dto.Description.Length < 10)
                return ServiceResponse<string>.Fail("Description must be at least 10 characters");

            //Check Lawyer exists
            var lawyer = await lawyerRepo.GetByIdAsync(dto.LawyerId);
            if (lawyer == null) 
                return ServiceResponse<string>.Fail("Lawyer not found");
            

            //Safety 
            if (dto.LawyerId == userId)
                return ServiceResponse<string>.Fail("You can't assign case to yourself");

            //Check for existing pending cases
            var hasPendingCase = (await caseRepo.GetAsync(c =>
                c.CustomerId == userId &&
                c.LawyerId == dto.LawyerId &&
                c.Status == CaseStatus.Pending
            )).Any();

            if (hasPendingCase)
                return ServiceResponse<string>.Fail("You already sent a request to this lawyer");

            //Create Case
            var caseRequest = new CaseRequest
            {
                CustomerId = userId,
                LawyerId = dto.LawyerId,
                Case_Type = caseType,
                Description = dto.Description,
                Status = CaseStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Documents = dto.Documents?
                    .Where(d => !string.IsNullOrWhiteSpace(d))
                    .Select(doc => new CaseDocument { FileUrl = doc.Trim() })
                    .ToList() ?? new List<CaseDocument>()
            };
      

            await caseRepo.AddAsync(caseRequest);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<string>.SuccessResult("Case created successfully");
        }


        public async Task<ServiceResponse<List<LawyerCaseRequestDto>>> GetRequestsForLawyerAsync(int lawyerId, string? status)
        {
            var caseRepo = _unitOfWork.GetRepository<CaseRequest, int>();
            

            Expression<Func<CaseRequest, bool>> filter = c => c.LawyerId == lawyerId;
            if (!string.IsNullOrEmpty(status))
            {
                if (!EnumHelper.TryParseEnum<CaseStatus>(status, out var statusEnum))
                    return ServiceResponse<List<LawyerCaseRequestDto>>.Fail("Invalid status");

                filter = c => c.LawyerId == lawyerId && c.Status == statusEnum;
            }

            var cases = await caseRepo.GetAsync(
                   filter,
                   q => q.OrderByDescending(c => c.CreatedAt),
                   null,
                   null,
                   c => c.Customer,
                   c => c.Documents
                 );

            var result = cases.Select(c => new LawyerCaseRequestDto
            {
                CaseId = c.Id,
                ClientName = $"{c.Customer.Fname} {c.Customer.Lname}",
                CaseType = c.Case_Type.ToString(),
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                Documents = c.Documents.Select(d => d.FileUrl).ToList()
            }).ToList();

            return ServiceResponse<List<LawyerCaseRequestDto>>.SuccessResult(result);
        }

        public async Task<ServiceResponse<string>> HandleCaseRequestAsync(int lawyerId, HandleCaseRequestDto dto)
        {
            var caseRepo = _unitOfWork.GetRepository<CaseRequest, int>();

            var caseRequest = (await caseRepo.GetAsync(
                c => c.Id == dto.CaseId,
                null,
                null,
                null,
                c => c.Lawyer 
            )).FirstOrDefault();

            // لو مش موجود
            if (caseRequest == null)
                return ServiceResponse<string>.Fail("Case not found");

            // لو الطلب مش بتاع المحامي ده
            if (caseRequest.LawyerId != lawyerId)
                return ServiceResponse<string>.Fail("Unauthorized action");

            //لو اترد عليه قبل كده
            if (caseRequest.Status != CaseStatus.Pending)
                return ServiceResponse<string>.Fail("Case already handled");

            // Accept
            if (dto.IsAccepted)
            {
                caseRequest.Status = CaseStatus.Accepted;
                await _notificationService.AddNotificationAsync(
                    caseRequest.CustomerId,
                    "Request Accepted",
                    $"Your request has been accepted by Dr. {caseRequest.Lawyer.Fname} {caseRequest.Lawyer.Lname}.",
                    NotificationType.Success,
                    caseRequest.Id
                );
                //  هنا بعدين هنفتح Chat
                // ممكن تعمل CreateChat هنا
            }
            else
            {
                //  Reject
                caseRequest.Status = CaseStatus.Rejected;
                await _notificationService.AddNotificationAsync(
                    caseRequest.CustomerId,
                    "Request Declined",
                    "Your request was declined. Please try another lawyer.",
                    NotificationType.Error,
                    caseRequest.Id
                );
            }

            

            caseRepo.Update(caseRequest);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<string>.SuccessResult(
                dto.IsAccepted ? "Case accepted successfully" : "Case rejected"
            );
        }

        public async Task<ServiceResponse<List<CustomerCaseDto>>> GetCustomerCasesAsync(int customerId)
        {
            var caseRepo = _unitOfWork.GetRepository<CaseRequest, int>();

            var cases = await caseRepo.GetAsync(
                    c => c.CustomerId == customerId,
                    orderBy: q => q.OrderByDescending(c => c.CreatedAt),
                    null,
                    null,
                    c => c.Lawyer
                );

            var result = cases.Select(c => new CustomerCaseDto
            {
                CaseId = c.Id,
                LawyerName = c.Lawyer != null ? $"{c.Lawyer.Fname} {c.Lawyer.Lname}" : null,
                CaseType = c.Case_Type.ToString(),
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt
            }).ToList();

            return ServiceResponse<List<CustomerCaseDto>>.SuccessResult(result);
        }

        public async Task<ServiceResponse<CaseDetailsDto>> GetCaseDetailsForLawyerAsync(int caseId, int lawyerId)
        {
            var caseRepo = _unitOfWork.GetRepository<CaseRequest, int>();

            var caseRequest = (await caseRepo.GetAsync(
                c => c.Id == caseId && c.LawyerId == lawyerId,
                null,
                null,
                null,
                c => c.Customer,
                c => c.Documents
            )).FirstOrDefault();

            if (caseRequest == null)
                return ServiceResponse<CaseDetailsDto>.Fail("Case not found");

            var result = new CaseDetailsDto
            {
                CaseId = caseRequest.Id,
                ClientName = $"{caseRequest.Customer.Fname} {caseRequest.Customer.Lname}",
                CaseType = caseRequest.Case_Type.ToString(),
                Description = caseRequest.Description,
                Status = caseRequest.Status.ToString(),
                CreatedAt = caseRequest.CreatedAt,

                Documents = caseRequest.Documents
                    .Select(d => d.FileUrl)
                    .ToList()
            };

            return ServiceResponse<CaseDetailsDto>.SuccessResult(result);
        }

        public async Task<ServiceResponse<object>> GetCaseDetailsAsync(int caseId, int userId)
        {
            var caseRepo = _unitOfWork.GetRepository<CaseRequest, int>();

            var caseRequest = (await caseRepo.GetAsync(
                c => c.Id == caseId,
                null,
                null,
                null,
                c => c.Lawyer,
                c => c.Customer
            )).FirstOrDefault();

            if (caseRequest == null)
                return ServiceResponse<object>.Fail("Case not found");

            // 🔥 أهم سطر
            if (caseRequest.LawyerId != userId && caseRequest.CustomerId != userId)
                return ServiceResponse<object>.Fail("Unauthorized");

            return ServiceResponse<object>.SuccessResult(new
            {
                caseRequest.Id,
                caseRequest.Description,
                caseRequest.Status,
                caseRequest.CreatedAt,

                Lawyer = $"{caseRequest.Lawyer.Fname} {caseRequest.Lawyer.Lname}",
                Customer = $"{caseRequest.Customer.Fname} {caseRequest.Customer.Lname}"
            });
        }

    }
}
