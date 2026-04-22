using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service_Abstraction
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName);
    }
}
