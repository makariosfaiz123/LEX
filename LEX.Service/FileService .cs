using LEX.Service_Abstraction;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEX.Service
{
    public class FileService : IFileService
    {
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            var newFileName = Guid.NewGuid() + Path.GetExtension(fileName);

            var path = Path.Combine("wwwroot/uploads", newFileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            return $"https://yourdomain.com/uploads/{newFileName}";
        }
    }
}
