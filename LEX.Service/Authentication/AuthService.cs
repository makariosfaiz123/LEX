using BCrypt.Net;
using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service.Helpers;
using LEX.Service_Abstraction;
using LEX.Service_Abstraction.Authentication;
using LEX.Service_Abstraction.IAuthentication;
using LEX.Service_Abstraction.ISecurity;
using LEX.Shared.DTOs.AuthenticationForgotPassword;
using LEX.Shared.DTOs.AuthenticationLogin;
using LEX.Shared.DTOs.AuthenticationRegister;
using LEX.Shared.DTOs.EmailOTP;
using LEX.Shared.Responses;
using Microsoft.AspNetCore.Http;
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
        private readonly IFileService _fileService;

        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IEmailService emailService, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _emailService = emailService;
            _fileService = fileService;
        }
        public async Task<ServiceResponse<RegisterResponseDto>> RegisterCustomerAsync(RegisterCustomerDto dto)
        {
            var userRepo = _unitOfWork.GetRepository<Users, int>();
            var existingUser = (await userRepo.GetAsync(u => u.Email == dto.Email)).FirstOrDefault();

            if (existingUser != null && existingUser.IsVerified)
                return ServiceResponse<RegisterResponseDto>.Fail("Email already exists");

            string baseUrl = "https://localhost:5232"; // غيره برابط الـ API بتاعك
            string verificationToken = Guid.NewGuid().ToString("N");

            // إعادة إرسال اللينك لو الحساب مش متفعل
            if (existingUser != null && !existingUser.IsVerified)
            {
                existingUser.OtpCode = verificationToken;
                existingUser.OtpExpiry = DateTime.UtcNow.AddMinutes(20); // ⏱️ 20 دقيقة
                await _unitOfWork.SaveChangesAsync();

                string resendLink = $"{baseUrl}/api/auth/verify-email?userId={existingUser.Id}&token={verificationToken}";
                await _emailService.SendEmailAsync(existingUser.Email, "LEX - Verify Your Email", GetVerificationEmailBody(resendLink));

                return ServiceResponse<RegisterResponseDto>.SuccessResult(new RegisterResponseDto { Message = "Verification link sent again.", UserId = existingUser.Id });
            }

            var customer = new Customer
            {
                Fname = dto.Fname,
                Lname = dto.Lname,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                //Date_Of_Birth = (DateOnly)dto.DateOfBirth,
                //gender = (Gender)dto.Gender,
                // Porfile_Image = dto.ProfileImage,
                RoleS = RoleS.Customer,
                Status = Status.Waiting,
                OtpCode = verificationToken,
                OtpExpiry = DateTime.UtcNow.AddMinutes(20), // ⏱️ 20 دقيقة
                IsVerified = false
            };

            await userRepo.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            string verifyLink = $"{baseUrl}/api/auth/verify-email?userId={customer.Id}&token={verificationToken}";
            await _emailService.SendEmailAsync(customer.Email, "LEX - Verify Your Email", GetVerificationEmailBody(verifyLink));

            return ServiceResponse<RegisterResponseDto>.SuccessResult(new RegisterResponseDto { Message = "Customer Registered. Check email.", UserId = customer.Id });
        }

        public async Task<ServiceResponse<RegisterResponseDto>> RegisterLawyerAsync(
            RegisterLawyerFormDto dto, List<(Stream Stream, string FileName, DocumentType Type)> documents)
        {
            var userRepo = _unitOfWork.GetRepository<Users, int>();
            var docRepo = _unitOfWork.GetRepository<Documents, int>();
            var baseUrl = "https://localhost:5232";

            var existingUser = (await userRepo.GetAsync(u => u.Email == dto.Email)).FirstOrDefault();
            if (existingUser != null && existingUser.IsVerified)
                return ServiceResponse<RegisterResponseDto>.Fail("Email already exists");

            string verificationToken = Guid.NewGuid().ToString("N");

            if (existingUser != null && !existingUser.IsVerified)
            {
                existingUser.OtpCode = verificationToken;
                existingUser.OtpExpiry = DateTime.UtcNow.AddMinutes(20); // ⏱️ 20 دقيقة
                await _unitOfWork.SaveChangesAsync();

                string resendLink = $"{baseUrl}/api/auth/verify-email?userId={existingUser.Id}&token={verificationToken}";
                await _emailService.SendEmailAsync(existingUser.Email, "LEX - Verify Your Email", GetVerificationEmailBody(resendLink));
                return ServiceResponse<RegisterResponseDto>.SuccessResult(new RegisterResponseDto { Message = "Verification link sent again.", UserId = existingUser.Id });
            }

            // فحص الملفات
            var allowedExtensions = new[] { ".pdf", ".jpeg", ".jpg", ".png" };
            foreach (var doc in documents)
            {
                if (!allowedExtensions.Contains(Path.GetExtension(doc.FileName).ToLower()))
                    return ServiceResponse<RegisterResponseDto>.Fail($"Invalid file type for {doc.Type}.");
                if (doc.Stream.Length > 5 * 1024 * 1024)
                    return ServiceResponse<RegisterResponseDto>.Fail("File too large (max 5MB)");
            }

            Specializations specs = Specializations.None;
            foreach (var spec in dto.Specializations)
            {
                if (Enum.TryParse<Specializations>(spec, out var parsed)) specs |= parsed;
            }

            var lawyer = new Lawyer
            {
                Fname = dto.Fname,
                Lname = dto.Lname,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleS = RoleS.Lawyer,
                Status = Status.Waiting,
                Start_Practice = dto.StartPractice.Value,
                Specialization = specs,
                OtpCode = verificationToken,
                OtpExpiry = DateTime.UtcNow.AddMinutes(20), // ⏱️ 20 دقيقة
                IsVerified = false
            };

            await userRepo.AddAsync(lawyer);
            await _unitOfWork.SaveChangesAsync();

            // رفع الملفات
            foreach (var doc in documents)
            {
                var url = await _fileService.UploadFileAsync(doc.Stream, doc.FileName);
                await docRepo.AddAsync(new Documents { Lawyer_Id = lawyer.Id, Document_URL = url, Document_Type = doc.Type });
            }
            await _unitOfWork.SaveChangesAsync();

            string verifyLink = $"{baseUrl}/api/auth/verify-email?userId={lawyer.Id}&token={verificationToken}";
            await _emailService.SendEmailAsync(lawyer.Email, "LEX - Verify Your Email", GetVerificationEmailBody(verifyLink));

            return ServiceResponse<RegisterResponseDto>.SuccessResult(new RegisterResponseDto { Message = "Lawyer Registered. Check email.", UserId = lawyer.Id });
        }

        public async Task<ServiceResponse<string>> VerifyEmailLinkAsync(int userId, string token)
        {
            var userRepo = _unitOfWork.GetRepository<Users, int>();
            var user = await userRepo.GetByIdAsync(userId);

            if (user == null) return ServiceResponse<string>.Fail("User not found.");
            if (user.IsVerified) return ServiceResponse<string>.Fail("Email already verified.");
            if (user.OtpCode != token) return ServiceResponse<string>.Fail("Invalid token.");
            if (user.OtpExpiry < DateTime.UtcNow) return ServiceResponse<string>.Fail("Link expired.");

            user.IsVerified = true;
            user.OtpCode = null;
            user.OtpExpiry = null;

            userRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<string>.SuccessResult("Email verified successfully.");
        }

        private string GetVerificationEmailBody(string link)
        {
            return $@"
            <html>
            <body style='font-family: Arial; padding: 20px;'>
                <div style='max-width: 500px; margin: auto; padding: 20px; border-radius: 10px; text-align: center;'>
                    <h2 style='color: #2c3e50;'>LEX ⚖️</h2>
                    <p>Click below to verify your email (Valid for 20 minutes):</p>
                    <a href='{link}' style='padding: 12px 24px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px;'>Verify Email</a>
                </div>
            </body>
            </html>";
        }
        public async Task<ServiceResponse<RegisterResponseDto>> RegisterAsync(RegisterDto dto)
        {
            var userRepo = _unitOfWork.GetRepository<Users, int>();

            // Check Email Exists
            var existingUser = (await userRepo.GetAsync(u => u.Email == dto.Email))
                               .FirstOrDefault();

            if (existingUser != null && existingUser.IsVerified)
                return ServiceResponse<RegisterResponseDto>.Fail("Email already exists");


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

                return ServiceResponse<RegisterResponseDto>.SuccessResult(
                    new RegisterResponseDto
                    {
                        Message = "OTP sent again. Check your email.",
                        UserId = existingUser.Id
                    }
                );
            }

            if (dto.DateOfBirth >= DateOnly.FromDateTime(DateTime.Now))
                return ServiceResponse<RegisterResponseDto>.Fail("Invalid Date of Birth");

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
                if (dto.StartPractice == null || dto.Specializations == null || !dto.Specializations.Any())
                    return ServiceResponse<RegisterResponseDto>.Fail("Lawyer data is incomplete");

                //Validation
                if (dto.Specializations.Count > 2)
                    return ServiceResponse<RegisterResponseDto>.Fail("Maximum 2 specializations allowed");

                // Convert List → Flags
                Specializations specs = Specializations.None;

                foreach (var spec in dto.Specializations)
                {
                    if (!EnumHelper.TryParseEnum<Specializations>(spec, out var parsed))
                        return ServiceResponse<RegisterResponseDto>.Fail($"Invalid specialization: {spec}");

                    specs |= parsed;
                }

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
                    Specialization = specs,
                };
            }
            else
            {
                return ServiceResponse<RegisterResponseDto>.Fail("Invalid Role");
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

            return ServiceResponse<RegisterResponseDto>.SuccessResult(
                new RegisterResponseDto
                {
                    Message = "User Registered. Check your email for OTP.",
                    UserId = user.Id
                }
            );
        }

        public async Task<ServiceResponse<string>> RegisterDocumentsAsync(
                int lawyerId,
                List<(Stream Stream, string FileName, DocumentType Type)> documents
            )
        {
            // Validation
            if (documents == null || !documents.Any())
                return ServiceResponse<string>.Fail("No documents uploaded");


            var docRepo = _unitOfWork.GetRepository<Documents, int>();

            var requiredTypes = new[]
            {
                DocumentType.Passport,
                DocumentType.SyndicateCard,
                DocumentType.LawyerLicense
            };

            var uploadedTypes = documents.Select(d => d.Type).Distinct().ToList();

            foreach (var required in requiredTypes)
            {
                if (!uploadedTypes.Contains(required))
                    return ServiceResponse<string>.Fail($"Missing required document: {required}");
            }

            // Allowed extensions
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };

            foreach (var doc in documents)
            {
                var extension = Path.GetExtension(doc.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    return ServiceResponse<string>.Fail("Only PDF/DOC/DOCX allowed");

                if (doc.Stream.Length > 5 * 1024 * 1024)
                    return ServiceResponse<string>.Fail("File too large (max 5MB)");

                var url = await _fileService.UploadFileAsync(doc.Stream, doc.FileName);

                await docRepo.AddAsync(new Documents
                {
                    Lawyer_Id = lawyerId,
                    Document_URL = url,
                    Document_Type = doc.Type
                });
            }

            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<string>.SuccessResult("Documents uploaded successfully");
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
                Token = token,
                Role = user.RoleS.ToString() // كده هترجع "Lawyer" أو "Customer"            };
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

        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var userRepo = _unitOfWork.GetRepository<Users, int>();

            var user = (await userRepo.GetAsync(u => u.Email == dto.Email))
                       .FirstOrDefault();

            if (user == null)
                return "User not found";

            if (!user.IsVerified)
                return "Email not verified";

            var otp = GenerateOtp();
            user.OtpCode = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5);

            await _unitOfWork.SaveChangesAsync();

            var body = $@"
                <html>
                <body style='font-family: Arial; background-color: #f4f4f4; padding: 20px;'>
                    <div style='max-width: 500px; margin: auto; background: white; padding: 20px; border-radius: 10px; text-align: center;'>
                        <h2 style='color: #2c3e50;'>LEX ⚖️</h2>
                        <p>Password Reset Code:</p>
                        <div style='font-size: 30px; font-weight: bold;'>{otp}</div>
                        <p>Expires in 5 minutes</p>
                    </div>
                </body>
                </html>";

            await _emailService.SendEmailAsync(
                user.Email,
                "LEX - Reset Password OTP",
                body
            );

            return "OTP sent to your email";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var userRepo = _unitOfWork.GetRepository<Users, int>();

            var user = (await userRepo.GetAsync(u => u.Email == dto.Email))
                       .FirstOrDefault();

            if (user == null)
                return "User not found";

            if (!user.IsVerified)
                return "Email not verified";

            //Validate OTP
            if (!string.Equals(user.OtpCode?.Trim(), dto.OtpCode?.Trim()))
                return "Invalid OTP";

            if (user.OtpExpiry == null || user.OtpExpiry < DateTime.UtcNow)
                return "OTP expired";

            //Update Password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            //Clear OTP
            user.OtpCode = null;
            user.OtpExpiry = null;

            await _unitOfWork.SaveChangesAsync();

            return "Password reset successfully";
        }

    }
}
