using System.IO;

namespace ZoDream.Shared.Models
{
    public class MoveFileItem : FileInfoItem
    {
        public string TargetName { get; set; } = string.Empty;
        public string TargetFileName { get; set; } = string.Empty;


        public void MoveTo(string fileName)
        {
            TargetFileName = fileName;
            TargetName = Path.GetFileName(fileName);
        }

        public MoveFileItem(string fileName) : base(fileName)
        {
        }

        public MoveFileItem(FileInfo info) : base(info)
        {
        }

        public MoveFileItem(DirectoryInfo info) : base(info)
        {
        }
    }
}
