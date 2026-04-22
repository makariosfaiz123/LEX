using LEX.Domain.Entities;
using LEX.Domain.EnumS;
using LEX.Domain.Interfaces;
using LEX.Service;
using LEX.Service_Abstraction;
using LEX.Shared.DTOs;
using LEX.Shared.DTOs.ProfileDto;
using LEX.Web.UploadProfailDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LEX.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public UsersController(IUnitOfWork unitOfWork ,IUserService userService,IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _fileService = fileService;
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateUserProfileRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userRepo = _unitOfWork.GetRepository<Users, int>();
            var user = await userRepo.GetByIdAsync(userId);

            if (user == null)
                return NotFound();

            bool isLawyer = user is Lawyer;

            //  upload image
            string? imageUrl = null;
            if (request.ProfileImage != null)
            {
                imageUrl = await _fileService.UploadFileAsync(
                    request.ProfileImage.OpenReadStream(),
                    request.ProfileImage.FileName
                );
            }

            // upload documents
            if (isLawyer && request.Documents != null && request.Documents.Any())
            {
                if (request.DocumentTypes == null ||
                    request.DocumentTypes.Count != request.Documents.Count)
                {
                    return BadRequest("Each document must have a type");
                }

                var docRepo = _unitOfWork.GetRepository<Documents, int>();

                for (int i = 0; i < request.Documents.Count; i++)
                {
                    var file = request.Documents[i];
                    var type = request.DocumentTypes[i];

                    var ext = Path.GetExtension(file.FileName).ToLower();

                    if (ext == ".jpg" || ext == ".png" || ext == ".jpeg")
                        return BadRequest("Images not allowed");

                    var url = await _fileService.UploadFileAsync(
                        file.OpenReadStream(),
                        file.FileName
                    );

                    await docRepo.AddAsync(new Documents
                    {
                        Lawyer_Id = userId,
                        Document_URL = url,
                        Document_Type = type 
                    });
                }

                await _unitOfWork.SaveChangesAsync();
            }

            //  DTO
            var dto = new UpdateUserProfileDto
            {
                Fname = request.Fname,
                Lname = request.Lname,
                Phone = request.Phone,
                About = isLawyer ? request.About : null,
                Education = isLawyer ? request.Education : null,
                ProfileImageUrl = imageUrl
            };

            var result = await _userService.UpdateProfileAsync(userId, dto);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _userService.GetMyProfileAsync(userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
