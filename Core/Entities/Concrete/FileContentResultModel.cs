namespace Core.Entities.Concrete
{
    public class FileContentResultModel : IEntity
    {
        public byte[] FileContents { get; set; }
        public string ContentType { get; set; }
        public string FileDownloadName { get; set; }
    }
}