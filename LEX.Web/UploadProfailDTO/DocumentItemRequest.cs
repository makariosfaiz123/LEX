using LEX.Domain.EnumS;

namespace LEX.Web.UploadProfailDTO
{
    public class DocumentItemRequest
    {
        public IFormFile File { get; set; }
        public DocumentType Type { get; set; }
    }
}
