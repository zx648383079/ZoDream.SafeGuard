using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.SafeGuard.Models
{
    public class FileTransformItem : BindableBase
    {

        public string Name { get; set; }

        public string FileName { get; set; }

        private FileTransformStatus status;

        public FileTransformStatus Status {
            get => status;
            set => Set(ref status, value);
        }

        public FileTransformItem(string fileName)
        {
            FileName = fileName;
            Name = Path.GetFileName(fileName);
        }

        public FileTransformItem(string name, string fileName, FileTransformStatus status)
        {
            Name = name;
            FileName = fileName;
            Status = status;
        }
    }
}
