
using AuthServer.Application.Common.Interfaces.ContentFiles;

namespace AuthServer.API.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public string WebRootPath => _hostingEnvironment.WebRootPath;

        public FileService(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public string GetEmailTemplateFile(string filename)
        {
            string templateFilePath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, "Content/EmailTemplates", filename);

            if (!File.Exists(templateFilePath))
            {
                throw new FileNotFoundException($"Template file '{filename}' not found in EmailTemplates directory.");
            }

            return File.ReadAllText(templateFilePath);
        }
    }
}
