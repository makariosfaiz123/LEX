using LEX.Domain.EnumS;
using Microsoft.AspNetCore.Http;

namespace LEX.Web.UploadProfailDTO
{
    public class UpdateUserProfileRequest
    {
        public string? Fname { get; set; }
        public string? Lname { get; set; }
        public string? Phone { get; set; }

        public string? About { get; set; }
        public string? Education { get; set; }

        public IFormFile? ProfileImage { get; set; }
        public List<IFormFile>? Documents { get; set; }

        public List<DocumentType>? DocumentTypes { get; set; }
    }
}
