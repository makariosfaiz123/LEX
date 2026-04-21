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

            // ⭐ Rating
            double rating = 0;
            int reviewsCount = lawyer.Reviews.Count;

            if (reviewsCount > 0)
                rating = lawyer.Reviews.Average(r => r.Rating);

            var result = new LawyerDetailsDto
            {
                Id = lawyer.Id,
                Name = $"{lawyer.Fname} {lawyer.Lname}",

                Specializations = EnumHelper.GetSpecializationsList(lawyer.Specialization),

                ProfileImage = lawyer.Porfile_Image,

                Rating = Math.Round(rating, 1),
                ReviewsCount = reviewsCount,

                YearsOfExperience = DateTime.Now.Year - lawyer.Start_Practice.Year
                 - (DateTime.Now.DayOfYear < lawyer.Start_Practice.DayOfYear ? 1 : 0),

                About = lawyer.About,
                Education = lawyer.Education,

                Reviews = lawyer.Reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    CustomerName = $"{r.Customer.Fname} {r.Customer.Lname}",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                }).ToList()
            };

            return ServiceResponse<LawyerDetailsDto>.SuccessResult(result);
        }

    }
}
