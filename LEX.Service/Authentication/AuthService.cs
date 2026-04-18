using BCrypt.Net;
using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service_Abstraction.Authentication;
using LEX.Service_Abstraction.IAuthentication;
using LEX.Service_Abstraction.ISecurity;
using LEX.Shared.DTOs.AuthenticationLogin;
using LEX.Shared.DTOs.AuthenticationRegister;
using LEX.Shared.DTOs.EmailOTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService ,IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var userRepo = _unitOfWork.GetRepository<Users, int>();

            // Check Email Exists
            var existingUser = (await userRepo.GetAsync(u => u.Email == dto.Email))
                               .FirstOrDefault();

            if (existingUser != null && existingUser.IsVerified)
                return "Email already exists";

            if (dto.DateOfBirth >= DateOnly.FromDateTime(DateTime.Now))
                return "Invalid Date of Birth";

            // Hash Password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Create User
            Users user;

            if (dto.Role == RoleS.Customer)
            {
                user = new Customer
                {
                    Fname = dto.Fname,
                    Lname = dto.Lname,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    PasswordHash = hashedPassword,
                    Date_Of_Birth = dto.DateOfBirth,
                    gender = dto.Gender,
                    Porfile_Image = dto.ProfileImage,
                    RoleS = dto.Role,
                    Status = Status.Active,
                };
            }
            else if (dto.Role == RoleS.Lawyer)
            {
                if (dto.StartPractice == null || dto.Specialization == null)
                    return "Lawyer data is incomplete";

                user = new Lawyer
                {
                    Fname = dto.Fname,
                    Lname = dto.Lname,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    PasswordHash = hashedPassword,
                    Date_Of_Birth = dto.DateOfBirth,
                    gender = dto.Gender,
                    Porfile_Image = dto.ProfileImage,
                    RoleS = RoleS.Lawyer,
                    Status = Status.Waiting,
                    Start_Practice = dto.StartPractice.Value,
                    Specialization = dto.Specialization.Value,
                };
            }
            else
            {
                return "Invalid Role";
            }

            //OTP Email
            var otp = GenerateOtp();

            user.OtpCode = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5);
            user.IsVerified = false;



            await userRepo.AddAsync(user);

            await _unitOfWork.SaveChangesAsync();

            //OTP Email
            await _emailService.SendEmailAsync(
                    user.Email,
                    "Your OTP Code",
                    $"Your OTP is: {otp}"
                );

            return "User Registered. Check your email for OTP.";
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var userRepo = _unitOfWork.GetRepository<Users, int>();

            var user = (await userRepo.GetAsync(u => u.Email == dto.Email))
                       .FirstOrDefault();

            if (user == null)
                return null;

            if (!user.IsVerified)
                return null;

            var isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isValid)
                return null;


            var token = _tokenService.CreateToken(user);

            return new AuthResponseDto
            {
                Email = user.Email,
                Token = token
            };
        }

        public async Task<string> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var userRepo = _unitOfWork.GetRepository<Users, int>();

            var user = (await userRepo.GetAsync(u => u.Email == dto.Email)).FirstOrDefault();

            if (user == null)
                return "User not found";


            if (!string.Equals(user.OtpCode?.Trim(), dto.OtpCode?.Trim()))
                return "Invalid OTP";

            if (user.OtpExpiry < DateTime.UtcNow)
                return "OTP expired";

            user.IsVerified = true;
            user.OtpCode = null;

            await _unitOfWork.SaveChangesAsync();

            return "Email verified successfully";
        }

        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

    }
}
