using System.IO;

namespace ZoDream.Shared.Models
{
    public class RenameFileItem : FileInfoItem
    {
        public string RenameName { get; set; } = string.Empty;

        public RenameFileItem(string fileName) : base(fileName)
        {
        }

        public RenameFileItem(FileInfo info) : base(info)
        {
        }

        public RenameFileItem(DirectoryInfo info) : base(info)
        {
        }
    }
}
