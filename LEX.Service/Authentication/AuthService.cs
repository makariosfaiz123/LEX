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




            // 🔁 لو موجود بس مش مفعل → ابعت OTP تاني
            string otp;
            if (existingUser != null && !existingUser.IsVerified)
            {
                otp = GenerateOtp();

                existingUser.OtpCode = otp;
                existingUser.OtpExpiry = DateTime.UtcNow.AddMinutes(5);

                await _unitOfWork.SaveChangesAsync();

                var resendBody = $@"
                    <html>
                    <body style='font-family: Arial; background-color: #f4f4f4; padding: 20px;'>
                        <div style='max-width: 500px; margin: auto; background: white; padding: 20px; border-radius: 10px; text-align: center;'>
                            <h2 style='color: #2c3e50;'>LEX ⚖️</h2>
                            <p>Your new OTP code:</p>
                            <div style='font-size: 30px; font-weight: bold;'>{otp}</div>
                            <p>Expires in 5 minutes</p>
                        </div>
                    </body>
                    </html>";

                await _emailService.SendEmailAsync(
                    existingUser.Email,
                    "LEX - OTP Resend",
                    resendBody
                );

                return "OTP sent again. Check your email.";
            }

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

            otp = GenerateOtp();

            user.OtpCode = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5);
            user.IsVerified = false;


            await userRepo.AddAsync(user);

            await _unitOfWork.SaveChangesAsync();



            //OTP Email
            var body = $@"
                <html>
                <body style='font-family: Arial; background-color: #f4f4f4; padding: 20px;'>

                    <div style='max-width: 500px; margin: auto; background: white; padding: 20px; border-radius: 10px; text-align: center;'>

                        <h2 style='color: #2c3e50;'>LEX ⚖️</h2>

                        <p style='font-size: 16px; color: #555;'>
                            Welcome! Please use the verification code below:
                        </p>

                        <div style='font-size: 30px; font-weight: bold; letter-spacing: 5px; 
                                    background: #f1f1f1; padding: 15px; margin: 20px 0; border-radius: 8px;'>
                            {otp}
                        </div>

                        <p style='color: #888; font-size: 14px;'>
                            This code will expire in 5 minutes.
                        </p>

                        <hr />

                        <p style='font-size: 12px; color: #aaa;'>
                            If you didn’t request this, you can ignore this email.
                        </p>

                    </div>

                </body>
                </html>
                ";



            // 📧 Send Email
            await _emailService.SendEmailAsync(
                user.Email,
                "LEX - OTP Verification",
                body
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


            if (user.IsVerified)
                return "Already verified";

            if (!string.Equals(user.OtpCode?.Trim(), dto.OtpCode?.Trim()))
                return "Invalid OTP";


            if (user.OtpExpiry == null || user.OtpExpiry < DateTime.UtcNow)
                return "OTP expired";

            user.IsVerified = true;
            user.OtpCode = null;

            user.OtpExpiry = null;

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
