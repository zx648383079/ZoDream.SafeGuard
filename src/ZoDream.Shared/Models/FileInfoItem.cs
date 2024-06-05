namespace ZoDream.Shared.Models
{
    public class FileInfoItem
    {

        public string Name { get; set; }

        public string FileName { get; set; }
        public string Extension { get; set; }

        public bool IsFolder { get; set; }

        public long Length { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public FileInfoItem(string fileName)
        {
            FileName = fileName;
            Name = Path.GetFileName(fileName);
            var info = new FileInfo(fileName);
            IsFolder = !info.Exists;
        }
    }
}
