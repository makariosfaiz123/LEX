using LEX.Service_Abstraction;
using LEX.Web.UploadProfailDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LEX.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        
        [Authorize(Roles = "Lawyer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _documentService.DeleteDocumentAsync(id, userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        
        [Authorize(Roles = "Lawyer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateDocumentRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _documentService.UpdateDocumentAsync(
                id,
                userId,
                request.File?.OpenReadStream(),
                request.File?.FileName,
                request.Type
            );

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Lawyer")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyDocuments()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _documentService.GetMyDocumentsAsync(userId);

            return Ok(result);
        }
    }
}
