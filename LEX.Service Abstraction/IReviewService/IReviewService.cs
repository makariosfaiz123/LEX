using LEX.Shared.DTOs.ReviewsDTOs;
using LEX.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service_Abstraction.IReviewService
{
    public interface IReviewService
    {
        Task<ServiceResponse<string>> AddReviewAsync(int customerId, CreateReviewDto dto);
        Task<ServiceResponse<string>> UpdateReviewAsync(int customerId, UpdateReviewDto dto);
        Task<ServiceResponse<string>> DeleteReviewAsync(int customerId, int reviewId);

    }
}
