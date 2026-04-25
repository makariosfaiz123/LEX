using LEX.Domain.EnumS;
using LEX.Shared.DTOs;
using LEX.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service_Abstraction
{
    public interface IDocumentService
    {
        Task<ServiceResponse<string>> DeleteDocumentAsync(int documentId, int userId);

        Task<ServiceResponse<string>> UpdateDocumentAsync(
            int documentId,
            int userId,
            Stream? fileStream,
            string? fileName,
            DocumentType? type
        );

        Task<ServiceResponse<List<DocumentDto>>> GetMyDocumentsAsync(int userId);


        Task<ServiceResponse<string>> ResubmitDocumentsAsync(
             int userId,
             List<(Stream Stream, string FileName)> files,
             List<DocumentType> types
         );
    }
}
