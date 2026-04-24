using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service.Helpers;
using LEX.Service_Abstraction.ILawyerService;
using LEX.Shared.DTOs.LawyerDtos;
using LEX.Shared.DTOs.ReviewsDTOs;
using LEX.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service.LawyerService
{
    public class LawyerService : ILawyerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILawyerRepository _lawyerRepo;

        public LawyerService(IUnitOfWork unitOfWork, ILawyerRepository lawyerRepository)
        {
            _unitOfWork = unitOfWork;
            _lawyerRepo = lawyerRepository;
        }
        public async Task<ServiceResponse<List<LawyerDto>>> GetLawyersAsync(LawyerFilterDto filter)
        {
            var lawyerRepo = _unitOfWork.GetRepository<Lawyer, int>();

            Expression<Func<Lawyer, bool>> predicate = l => true;

            //Filter specialization
            if (!string.IsNullOrEmpty(filter.Specialization)
                && filter.Specialization != "All")
            {
                if (!EnumHelper.TryParseEnum<Specializations>(
                    filter.Specialization, out var specializationEnum))
                {
                    return ServiceResponse<List<LawyerDto>>.Fail("Invalid specialization");
                }

                predicate = l => l.Specialization.HasFlag(specializationEnum);
            }

            var lawyers = await lawyerRepo.GetAsync(predicate);

            //Search (لسه في الذاكرة عادي)
            if (!string.IsNullOrEmpty(filter.Search))
            {
                lawyers = lawyers.Where(l =>
                    ($"{l.Fname} {l.Lname}")
                    .ToLower()
                    .Contains(filter.Search.ToLower()));
            }

            var result = lawyers.Select(l => new LawyerDto
            {
                Id = l.Id,
                Name = $"{l.Fname} {l.Lname}",
                Specializations = EnumHelper.GetSpecializationsList(l.Specialization),
                ProfileImage = l.Porfile_Image,

                Rating = 4.9, // مؤقت لحد ما نربط Reviews
                YearsOfExperience = DateTime.Now.Year - l.Start_Practice.Year,

            }).ToList();

            return ServiceResponse<List<LawyerDto>>.SuccessResult(result);
        }

        public async Task<ServiceResponse<List<SpecializationDto>>> GetSpecializationsAsync()
        {
            var lawyerRepo = _unitOfWork.GetRepository<Lawyer, int>();

            var lawyers = await lawyerRepo.GetAsync();


            var result = Enum.GetValues(typeof(Specializations))
                .Cast<Specializations>()
                .Where(s => s != Specializations.None)
                .Select(s => new SpecializationDto
                {
                    Name = s.ToString(),
                    Count = lawyers.Count(l => l.Specialization.HasFlag(s))
                })
                .ToList();


            result.Insert(0, new SpecializationDto
            {
                Name = "All",
                Count = lawyers.Count()
            });

            return ServiceResponse<List<SpecializationDto>>.SuccessResult(result);
        }
        public async Task<ServiceResponse<LawyerDetailsDto>> GetLawyerDetailsAsync(int lawyerId)
        {
            var lawyer = await _lawyerRepo.GetLawyerWithDetailsAsync(lawyerId);

            if (lawyer == null)
                return ServiceResponse<LawyerDetailsDto>.Fail("Lawyer not found");

            // 1. حساب التقييم بأمان
            var reviews = lawyer.Reviews ?? new List<Review>();
            double rating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

            // 2. حساب سنوات الخبرة بشكل متوافق مع DateOnly
            var today = DateOnly.FromDateTime(DateTime.Now);
            var experienceYears = today.Year - lawyer.Start_Practice.Year;

            // لو لسه مجاش يوم التخرج/بداية العمل في السنة الحالية، بنقص سنة
            if (today.DayOfYear < lawyer.Start_Practice.DayOfYear)
            {
                experienceYears--;
            }

            // 3. بناء الـ DTO
            var result = new LawyerDetailsDto
            {
                Id = lawyer.Id,
                Name = $"{lawyer.Fname} {lawyer.Lname}".Trim(),

                Specializations = EnumHelper.GetSpecializationsList(lawyer.Specialization),

                ProfileImage = lawyer.Porfile_Image,
                Rating = Math.Round(rating, 1),
                ReviewsCount = reviews.Count,

                // لو بالغلط تاريخ البداية كان في المستقبل، بنخليها 0
                YearsOfExperience = experienceYears < 0 ? 0 : experienceYears,

                About = lawyer.About,
                Education = lawyer.Education,

                // ==========================
                // Location FIX (Direct Access)
                // ==========================
                Street = lawyer.Location?.Street,
                City = lawyer.Location?.City?.Name,
                Governorate = lawyer.Location?.Governorate?.Name, // 💡 تعديل: وصول مباشر للمحافظة
                Country = lawyer.Location?.Country?.Name,         // 💡 تعديل: وصول مباشر للدولة

                // ==========================
                // Reviews Mapping
                // ==========================
                Reviews = reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    CustomerName = r.Customer != null
                        ? $"{r.Customer.Fname} {r.Customer.Lname}".Trim()
                        : "Unknown",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                }).ToList()
            };

            return ServiceResponse<LawyerDetailsDto>.SuccessResult(result);
        }
    }
}
