using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModels;

namespace ZoDream.SafeGuard.Models
{
    public class FileInfoItem
    {

        public string Name { get; set; }

        public string FileName { get; set; }

        public bool IsFolder { get; set; }

        public FileInfoItem(string fileName)
        {
            FileName = fileName;
            Name = Path.GetFileName(fileName);
            var info = new FileInfo(fileName);
            IsFolder = !info.Exists;
        }
    }
}
