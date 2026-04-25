using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service_Abstraction;
using LEX.Shared.DTOs;
using LEX.Shared.DTOs.ProfileDto;
using LEX.Shared.Responses;

namespace LEX.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly ILawyerRepository _lawyerRepository;

        public UserService(IUnitOfWork unitOfWork, IFileService fileService, ILawyerRepository lawyerRepository)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _lawyerRepository = lawyerRepository;
        }

        public async Task<ServiceResponse<string>> UpdateProfileAsync(int userId, UpdateUserProfileDto dto)
        {
            var userRepo = _unitOfWork.GetRepository<Users, int>();
            var user = await userRepo.GetByIdAsync(userId);

            if (user == null)
                return ServiceResponse<string>.Fail("User not found");

            if (!string.IsNullOrWhiteSpace(dto.Fname))
                user.Fname = dto.Fname;

            if (!string.IsNullOrWhiteSpace(dto.Lname))
                user.Lname = dto.Lname;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                user.Phone = dto.Phone;

            if (user is Lawyer lawyer)
            {
                if (!string.IsNullOrWhiteSpace(dto.About))
                    lawyer.About = dto.About;

                if (!string.IsNullOrWhiteSpace(dto.Education))
                    lawyer.Education = dto.Education;
            }

            if (!string.IsNullOrEmpty(dto.ProfileImageUrl))
                user.Porfile_Image = dto.ProfileImageUrl;

            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<string>.SuccessResult("Profile updated successfully");
        }

        public async Task<ServiceResponse<object>> GetMyProfileAsync(int userId)
        {
            var userRepo = _unitOfWork.GetRepository<Users, int>();

            var user = (await userRepo.GetAsync(u => u.Id == userId)).FirstOrDefault();

            if (user == null)
                return ServiceResponse<object>.Fail("User not found");

            // 🟢 Customer
            if (user.RoleS == RoleS.Customer)
            {
                var customerDto = new CustomerProfileDto
                {
                    Id = user.Id,
                    Name = $"{user.Fname} {user.Lname}",
                    Email = user.Email,
                    Phone = user.Phone,
                    ProfileImage = user.Porfile_Image,
                    Role = user.RoleS.ToString()
                };

                return ServiceResponse<object>.SuccessResult(customerDto);
            }

            // 🟢 Lawyer (Query منفصل 👇)
            if (user.RoleS == RoleS.Lawyer)
            {
                var lawyerRepo = _unitOfWork.GetRepository<Lawyer, int>();

                var lawyer = (await lawyerRepo.GetAsync(
                    l => l.Id == userId,
                    null,
                    null,
                    null,
                    l => l.Reviews,
                    l => l.Documents,
                    l => l.Location,
                    l => l.Location!.Country,
                    l => l.Location!.Governorate,
                    l => l.Location!.City
                )).FirstOrDefault();

                if (lawyer == null)
                    return ServiceResponse<object>.Fail("Lawyer not found");

                double rating = 0;
                int reviewsCount = lawyer.Reviews?.Count ?? 0;

                if (reviewsCount > 0)
                    rating = lawyer.Reviews!.Average(r => r.Rating);

                var lawyerDto = new LawyerProfileDto
                {
                    Id = lawyer.Id,
                    Name = $"{lawyer.Fname} {lawyer.Lname}",
                    Email = lawyer.Email,
                    Phone = lawyer.Phone,
                    ProfileImage = lawyer.Porfile_Image,
                    Role = lawyer.RoleS.ToString(),

                    About = lawyer.About,
                    Education = lawyer.Education,

                    YearsOfExperience = lawyer.Start_Practice.Year > 1
                        ? DateTime.Now.Year - lawyer.Start_Practice.Year
                        : 0,

                    Rating = Math.Round(rating, 1),
                    ReviewsCount = reviewsCount,

                    Country = lawyer.Location?.Country?.Name,
                    Governorate = lawyer.Location?.Governorate?.Name,
                    City = lawyer.Location?.City?.Name,
                    Street = lawyer.Location?.Street,

                    Documents = lawyer.Documents?.Select(d => new DocumentDto
                    {
                        Id = d.Id,
                        Url = d.Document_URL,
                        Type = d.Document_Type.ToString()
                    }).ToList() ?? new List<DocumentDto>()
                };

                return ServiceResponse<object>.SuccessResult(lawyerDto);
            }
            // احتياطي لو في Role غير معروف
            return ServiceResponse<object>.Fail("Invalid user role");
        }
    }
}
