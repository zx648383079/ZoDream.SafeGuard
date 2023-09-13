using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.SafeGuard.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.SafeGuard.Finders
{
    public class FileLoader: IDisposable
    {
        public FileInfo File { get; private set; }

        public FileCheckStatus Status { get; set; } = FileCheckStatus.Normal;

        public long Length => File.Length;
        public string Name => File.Name;

        private string? _md5;
        public string Md5 => _md5 ??= LocationStorage.GetMD5(File.FullName);

        private StreamReader? _reader;
        public StreamReader Reader => _reader ??= LocationStorage.Reader(File.FullName);

        public FileLoader(FileInfo info)
        {
            File = info;
        }

        public FileLoader(string fileName): this(new FileInfo(fileName))
        {
             
        }

        public void Dispose()
        {
            _reader?.Dispose();
        }

        public static void EachFiles(DirectoryInfo info, Action<FileInfo> action)
        {
            if (!info.Exists)
            {
                return;
            }
            foreach (var item in info.GetDirectories())
            {
                EachFiles(item, action);
            }
            foreach (var item in info.GetFiles())
            {
                action.Invoke(item);
            }
        }
    }
}
