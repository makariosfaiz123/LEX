using LEX.Domain.EnumS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LEX.Web.UploadProfailDTO
{

    public class RegisterDocumentsRequest
    {
        public int LawyerId { get; set; }
        public List<DocumentItemRequest> Documents { get; set; }
    }
}
