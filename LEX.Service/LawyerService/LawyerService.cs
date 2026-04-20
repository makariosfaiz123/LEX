using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service.Helpers;
using LEX.Service_Abstraction.ILawyerService;
using LEX.Shared.DTOs.LawyerDtos;
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

        public LawyerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

                predicate = l => l.Specialization == specializationEnum;
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
                Specialization = l.Specialization.ToString(),
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

            
            var result = lawyers
                .GroupBy(l => l.Specialization)
                .Select(g => new SpecializationDto
                {
                    Name = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToList();

            
            result.Insert(0, new SpecializationDto
            {
                Name = "All",
                Count = lawyers.Count()
            });

            return ServiceResponse<List<SpecializationDto>>.SuccessResult(result);
        }


    }
}
