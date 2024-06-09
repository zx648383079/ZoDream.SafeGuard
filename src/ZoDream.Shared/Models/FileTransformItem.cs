using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModels;

namespace ZoDream.Shared.Models
{
    public class FileTransformItem : BindableBase
    {

        public string Name { get; set; }

        public string FileName { get; set; }

        public bool IsFolder { get; set; }

        private string _renameName = string.Empty;
        /// <summary>
        /// 是否重改变了文件名
        /// </summary>
        public string RenameName {
            get => _renameName;
            set => Set(ref _renameName, value);
        }


        private FileTransformStatus _status;

        public FileTransformStatus Status {
            get => _status;
            set => Set(ref _status, value);
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

        public FileTransformItem(FileInfoItem file, FileTransformStatus status)
        {
            Name = file.Name;
            FileName = file.FileName;
            Status = status;
            IsFolder = file.IsFolder;
            if (file is RenameFileItem o)
            {
                RenameName = o.RenameName;
            }
        }
    }
}
