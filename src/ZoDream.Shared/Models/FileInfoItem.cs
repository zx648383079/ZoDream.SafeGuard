using ZoDream.Shared.Finders;

namespace ZoDream.Shared.Models
{
    public class FileInfoItem
    {

        public string Name { get; set; }

        public string FileName { get; set; }
        public string Extension { get; set; } = string.Empty;

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
            if (!IsFolder)
            {
                Extension = StorageFinder.GetExtension(fileName);
            }
        }

        public FileInfoItem(FileInfo info)
        {
            FileName = info.FullName;
            Name = info.Name;
            IsFolder = false;
            Extension = StorageFinder.GetExtension(info);
        }

        public FileInfoItem(DirectoryInfo info)
        {
            FileName = info.FullName;
            Name = info.Name;
            IsFolder = true;
        }
    }
}
