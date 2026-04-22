namespace LEX.Web.UploadProfailDTO
{
    using Microsoft.AspNetCore.Http;
    using LEX.Domain.EnumS;

    public class UpdateDocumentRequest
    {
        public IFormFile? File { get; set; } 
        public DocumentType? Type { get; set; } 
    }
}
