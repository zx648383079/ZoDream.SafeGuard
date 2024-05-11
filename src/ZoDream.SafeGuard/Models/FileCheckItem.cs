using System.IO;
using ZoDream.Shared.ViewModel;

namespace ZoDream.SafeGuard.Models
{
    public class FileCheckItem : BindableBase
    {

        public string Name { get; set; }

        public string FileName { get; set; }

        private FileCheckStatus status;

        public FileCheckStatus Status {
            get => status;
            set => Set(ref status, value);
        }

        public FileCheckItem(string fileName)
        {
            FileName = fileName;
            Name = Path.GetFileName(fileName);
        }

        public FileCheckItem(string name, string fileName, FileCheckStatus status)
        {
            Name = name;
            FileName = fileName;
            Status = status;
        }
    }
}
