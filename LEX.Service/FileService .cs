using LEX.Service_Abstraction;

namespace LEX.Service
{
    public class FileService : IFileService
    {
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            var newFileName = Guid.NewGuid() + Path.GetExtension(fileName);

            var folder = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, newFileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            return $"https://yourdomain.com/uploads/{newFileName}";
        }
    }
}
