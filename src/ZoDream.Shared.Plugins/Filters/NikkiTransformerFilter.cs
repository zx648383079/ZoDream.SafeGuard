using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ZoDream.Shared.Plugins.Filters
{
    public class NikkiTransformerFilter : ITransformerFilter
    {
        private readonly List<string> _includeItems = [];
        private readonly List<string> _excludeItems = [];
        private readonly string _extension = ".ktx";

        public void Ready()
        {
            _includeItems.Clear();
            _excludeItems.Clear();
        }

        public bool IsMatchFile(FileInfo fileInfo, CancellationToken token = default)
        {
            var ext = GetExtension(fileInfo.Name);
            if (_includeItems.Contains(ext))
            {
                return true;
            }
            if (_excludeItems.Contains(ext))
            {
                return false;
            }
            if (fileInfo.Length < 20)
            {
                return false;
            }
            if (!IsMatchFile(fileInfo.FullName))
            {
                _excludeItems.Add(ext);
                return false;
            }
            _includeItems.Add(ext);
            return true;
        }

        public string TranformFileName(string fileName)
        {
            var i = fileName.LastIndexOf('.');
            if (i < 0)
            {
                return fileName + _extension;
            }
            return fileName[..i] + _extension;
        }

        private bool IsMatchFile(string fileName)
        {
            using var fs = File.OpenRead(fileName);
            fs.Seek(-3, SeekOrigin.End);
            var buffer = new byte[3];
            fs.Read(buffer, 0, buffer.Length);
            return buffer[0] == 'p'
                && buffer[1] == 'n' && buffer[2] == 'g';
        }

        private string GetExtension(string fileName)
        {
            var i = fileName.IndexOf('.');
            return i < 0 ? string.Empty : fileName[(i + 1)..].ToLower();
        }
    }
}
