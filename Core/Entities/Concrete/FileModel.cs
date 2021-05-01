namespace Core.Entities.Concrete
{
    public class FileModel : IEntity
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}