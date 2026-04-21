using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service_Abstraction.IReviewService;
using LEX.Shared.DTOs.ReviewsDTOs;
using LEX.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service.ReviewService
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<string>> AddReviewAsync(int customerId, CreateReviewDto dto)
        {
            var reviewRepo = _unitOfWork.GetRepository<Review, int>();

            // Validation
            if (dto.Rating < 1 || dto.Rating > 5)
                return ServiceResponse<string>.Fail("Rating must be between 1 and 5");

            if (string.IsNullOrWhiteSpace(dto.Comment))
                dto.Comment = null;
            else
                dto.Comment = dto.Comment.Trim();

            var existing = (await reviewRepo.GetAsync(
                r => r.CustomerId == customerId && r.LawyerId == dto.LawyerId
            )).FirstOrDefault();

            if (existing != null)
                return ServiceResponse<string>.Fail("You already reviewed this lawyer");

            //var caseRepo = _unitOfWork.GetRepository<CaseRequest, int>();

            //var hasCompletedCase = (await caseRepo.GetAsync(
            //    c => c.CustomerId == customerId
            //      && c.LawyerId == dto.LawyerId
            //      && c.Status == CaseStatus.Completed
            //)).Any();

            //if (!hasCompletedCase)
            //    return ServiceResponse<string>.Fail("You can only review after completing a case");

            var review = new Review
            {
                CustomerId = customerId,
                LawyerId = dto.LawyerId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            await reviewRepo.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<string>.SuccessResult("Review added successfully");
        }

        public async Task<ServiceResponse<string>> UpdateReviewAsync(int customerId, UpdateReviewDto dto)
        {
            var reviewRepo = _unitOfWork.GetRepository<Review, int>();

            var review = (await reviewRepo.GetAsync(
                r => r.Id == dto.ReviewId && r.CustomerId == customerId
            )).FirstOrDefault();

            if (review == null)
                return ServiceResponse<string>.Fail("Review not found");

            if (dto.Rating < 1 || dto.Rating > 5)
                return ServiceResponse<string>.Fail("Rating must be between 1 and 5");

            review.Rating = dto.Rating;
            review.Comment = dto.Comment?.Trim();

            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<string>.SuccessResult("Review updated successfully");
        }

        public async Task<ServiceResponse<string>> DeleteReviewAsync(int customerId, int reviewId)
        {
            var reviewRepo = _unitOfWork.GetRepository<Review, int>();

            var review = (await reviewRepo.GetAsync(
                r => r.Id == reviewId && r.CustomerId == customerId
            )).FirstOrDefault();

            if (review == null)
                return ServiceResponse<string>.Fail("Review not found");

            reviewRepo.Delete(review);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<string>.SuccessResult("Review deleted successfully");
        }

    }
}
