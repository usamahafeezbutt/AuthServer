namespace AuthServer.Application.Common.Interfaces.ContentFiles
{
    public interface IFileService
    {
        public string WebRootPath { get; }
        string GetEmailTemplateFile(string filename);
    }
}
