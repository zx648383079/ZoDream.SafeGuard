using System;
using System.IO;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModels;

namespace ZoDream.SafeGuard.ViewModels
{
    public class RenameFileItemViewModel: BindableBase
    {
        public string Name { get; set; }

        public string FileName { get; set; }
        /// <summary>
        /// 包含 .
        /// </summary>
        public string Extension { get; set; } = string.Empty;

        public bool IsFolder { get; set; }

        private int _index;

        public int Index {
            get => _index;
            set => Set(ref _index, value);
        }


        private string _replaceName = string.Empty;

        public string ReplaceName {
            get => _replaceName;
            set => Set(ref _replaceName, value);
        }

        public DateTime CreatedAt { get; set; }
        public string NameWithoutExtension => string.IsNullOrWhiteSpace(Extension) ? Name : Name[..(Name.Length - Extension.Length)];


        public RenameFileItemViewModel(string fileName)
        {
            FileName = fileName;
            ReplaceName = Name = Path.GetFileName(fileName);
            var info = new FileInfo(fileName);
            IsFolder = !info.Exists;
            if (!IsFolder)
            {
                Extension = info.Extension;
                CreatedAt = info.CreationTime;
            } else
            {
                CreatedAt = Directory.GetCreationTime(fileName);
            }
        }

        public RenameFileItemViewModel(FileInfo info)
        {
            FileName = info.FullName;
            ReplaceName = Name = info.Name;
            IsFolder = false;
            Extension = info.Extension;
            CreatedAt = info.CreationTime;
        }

        public RenameFileItemViewModel(DirectoryInfo info)
        {
            FileName = info.FullName;
            ReplaceName = Name = info.Name;
            IsFolder = true;
            CreatedAt = info.CreationTime;
        }

        public RenameFileItemViewModel(FileInfoItem file)
        {
            ReplaceName = Name = file.Name;
            FileName = file.FileName;
            Extension = file.Extension;
            IsFolder = file.IsFolder;
            CreatedAt = file.CreatedAt;
        }
    }
}
