using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service.Helpers;
using LEX.Service_Abstraction;
using LEX.Service_Abstraction.CaseRequests;
using LEX.Shared.DTOs.CaseRequest;
using LEX.Shared.Responses;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _webHostEnvironment; // 🔥 ضيف ده
        public CaseRequestService(IUnitOfWork unitOfWork, INotificationService notificationService, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _webHostEnvironment = webHostEnvironment; // 🔥 ضيف ده
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ServiceResponse<string>> CreateCaseAsync(CreateCaseRequestDto dto, int userId)
        {
            var caseRepo = _unitOfWork.GetRepository<CaseRequest, int>();
            var lawyerRepo = _unitOfWork.GetRepository<Lawyer, int>();

            // Validate CaseType
            if (!EnumHelper.TryParseEnum<CaseType>(dto.CaseType, out var caseType))
                return ServiceResponse<string>.Fail("Invalid CaseType");

            if (string.IsNullOrWhiteSpace(dto.Description))
                return ServiceResponse<string>.Fail("Description is required");

            dto.Description = dto.Description.Trim();

            if (dto.Description.Length < 10)
                return ServiceResponse<string>.Fail("Description must be at least 10 characters");

            // Check Lawyer exists
            var lawyer = await lawyerRepo.GetByIdAsync(dto.LawyerId);
            if (lawyer == null)
                return ServiceResponse<string>.Fail("Lawyer not found");

            // Safety 
            if (dto.LawyerId == userId)
                return ServiceResponse<string>.Fail("You can't assign case to yourself");

            // Check for existing pending cases
            var hasPendingCase = (await caseRepo.GetAsync(c =>
                c.CustomerId == userId &&
                c.LawyerId == dto.LawyerId &&
                c.Status == CaseStatus.Pending
            )).Any();

            if (hasPendingCase)
                return ServiceResponse<string>.Fail("You already sent a request to this lawyer");

            // 🔥 1. معالجة وحفظ الملفات في wwwroot/uploads
            var documentEntities = new List<CaseDocument>();

            if (dto.Documents != null && dto.Documents.Any())
            {
                // تحديد مسار فولدر uploads
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                // لو الفولدر مش موجود، نكريته
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var file in dto.Documents)
                {
                    if (file.Length > 0)
                    {
                        // عمل اسم فريد للملف عشان ميمسحش ملفات قديمة
                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // حفظ الملف على الهارد ديسك
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        // إضافة المسار للستة الداتا بيز (هنحفظ المسار النسبي)
                        documentEntities.Add(new CaseDocument
                        {
                            FileUrl = $"/uploads/{uniqueFileName}"
                        });
                    }
                }
            }

            // 🔥 2. إنشاء الطلب بالملفات المرفوعة
            var caseRequest = new CaseRequest
            {
                CustomerId = userId,
                LawyerId = dto.LawyerId,
                Case_Type = caseType,
                Description = dto.Description,
                Status = CaseStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Documents = documentEntities // ربط الملفات هنا
            };

            await caseRepo.AddAsync(caseRequest);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<string>.SuccessResult("Case created successfully");
        }


        public async Task<ServiceResponse<List<LawyerCaseRequestDto>>> GetRequestsForLawyerAsync(int lawyerId, string? status, string? caseType)
        {
            var caseRepo = _unitOfWork.GetRepository<CaseRequest, int>();

            // 1. معالجة فلتر الـ Status (لو مبعوت)
            bool filterByStatus = false;
            CaseStatus statusEnum = default;
            if (!string.IsNullOrEmpty(status))
            {
                if (!EnumHelper.TryParseEnum<CaseStatus>(status, out statusEnum))
                    return ServiceResponse<List<LawyerCaseRequestDto>>.Fail("Invalid status");

                filterByStatus = true;
            }

            // 2. معالجة فلتر الـ CaseType (لو مبعوت)
            bool filterByCaseType = false;
            CaseType caseTypeEnum = default;
            if (!string.IsNullOrEmpty(caseType))
            {
                if (!EnumHelper.TryParseEnum<CaseType>(caseType, out caseTypeEnum))
                    return ServiceResponse<List<LawyerCaseRequestDto>>.Fail("Invalid case type");

                filterByCaseType = true;
            }

            // 3. بناء الفلتر المجمع
            Expression<Func<CaseRequest, bool>> filter = c =>
                c.LawyerId == lawyerId &&
                (!filterByStatus || c.Status == statusEnum) &&
                (!filterByCaseType || c.Case_Type == caseTypeEnum);

            // 4. جلب البيانات من الداتا بيز
            var cases = await caseRepo.GetAsync(
                filter,
                q => q.OrderByDescending(c => c.CreatedAt),
                null,
                null,
                c => c.Customer,
                c => c.Documents
            );

            // 5. تحويل البيانات لـ DTO
            var result = cases.Select(c => new LawyerCaseRequestDto
            {
                CaseId = c.Id,
                ClientName = $"{c.Customer.Fname} {c.Customer.Lname}",
                CaseType = c.Case_Type.ToString(),
                Status = c.Status.ToString(), // <-- Mapped the status here
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                Documents = c.Documents.Select(d => d.FileUrl).ToList()
            }).ToList();

            return ServiceResponse<List<LawyerCaseRequestDto>>.SuccessResult(result);
        }
        public async Task<ServiceResponse<string>> HandleCaseRequestAsync(int lawyerId, HandleCaseRequestDto dto)
        {
            var caseRepo = _unitOfWork.GetRepository<CaseRequest, int>();
            var conversationRepo = _unitOfWork.GetRepository<Conversation, int>(); // Get the Conversation repo

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

            string? firebaseChatId = null;

            // Accept
            if (dto.IsAccepted)
            {
                caseRequest.Status = CaseStatus.Accepted;

                // 1. Generate a unique ID for the Firebase chat room
                firebaseChatId = Guid.NewGuid().ToString();

                // 2. Create the Conversation entity
                var newConversation = new Conversation
                {
                    CaseRequestId = caseRequest.Id,
                    LawyerId = caseRequest.LawyerId,
                    CustomerId = caseRequest.CustomerId,
                    FirebaseChatId = firebaseChatId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // 3. Add to repository
                await conversationRepo.AddAsync(newConversation);

                await _notificationService.AddNotificationAsync(
                    caseRequest.CustomerId,
                    "Request Accepted",
                    $"Your request has been accepted by Dr. {caseRequest.Lawyer.Fname} {caseRequest.Lawyer.Lname}.",
                    NotificationType.Success
                );
            }
            else
            {
                // Reject
                caseRequest.Status = CaseStatus.Rejected;
                await _notificationService.AddNotificationAsync(
                    caseRequest.CustomerId,
                    "Request Declined",
                    "Your request was declined. Please try another lawyer.",
                    NotificationType.Error
                );
            }

            caseRepo.Update(caseRequest);
            await _unitOfWork.SaveChangesAsync();

            // Return the FirebaseChatId if accepted, so the mobile app can navigate straight to the chat
            return ServiceResponse<string>.SuccessResult(
                dto.IsAccepted ? firebaseChatId : "Case rejected"
            );
        }

        public async Task<ServiceResponse<DashboardSummaryDto>> GetLawyerDashboardSummaryAsync(int lawyerId)
        {
            try
            {
                var caseRepo = _unitOfWork.GetRepository<CaseRequest, int>();

                // 🔥 كل الكيسات الخاصة بالمحامي
                var cases = await caseRepo.GetAsync(c => c.LawyerId == lawyerId);

                // 1️⃣ عدد الطلبات الجديدة (Pending)
                int pendingCount = cases.Count(c => c.Status == CaseStatus.Pending);

                // 2️⃣ عدد الطلبات المقبولة
                int acceptedCount = cases.Count(c => c.Status == CaseStatus.Accepted);

                // 3️⃣ عدد الطلبات المرفوضة
                int rejectedCount = cases.Count(c => c.Status == CaseStatus.Rejected);

                // 4️⃣ إجمالي الطلبات
                int totalCases = cases.Count();

                // 5️⃣ عدد العملاء (Distinct)
                int clientsCount = cases
                    .Select(c => c.CustomerId)
                    .Distinct()
                    .Count();

                // 6️⃣ أحدث الطلبات (آخر 5)
                var recentCases = cases
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .Select(c => new
                    {
                        c.Id,
                        c.Description,
                        Status = c.Status.ToString(),
                        Type = c.Case_Type.ToString(),
                        c.CreatedAt
                    })
                    .ToList();

                var summary = new DashboardSummaryDto
                {
                    ClientsRequests = pendingCount,
                    TotalCases = totalCases,
                    AcceptedCases = acceptedCount,
                    RejectedCases = rejectedCount,
                    ClientsCount = clientsCount,
                    RecentCases = recentCases
                };

                return ServiceResponse<DashboardSummaryDto>.SuccessResult(summary);
            }
            catch (Exception ex)
            {
                return ServiceResponse<DashboardSummaryDto>.Fail(
                    $"Error loading dashboard summary: {ex.Message}"
                );
            }
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

    }
}
