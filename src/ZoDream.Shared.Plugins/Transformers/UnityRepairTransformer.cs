using System;
using System.IO;
using System.Threading;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Plugins.Transformers
{
    public class UnityRepairTransformer : StorageFinder, ITransformFinder
    {
        private readonly byte[] _tagBuffer = [0x55, 0x6E, 0x69, 0x74, 0x79];

        public event FinderEventHandler? FoundChanged;

        public void Add(IFileTransformer transformer)
        {
        }

        public void Clear()
        {
        }

        public string Transform(string content, CancellationToken token = default)
        {
            return content;
        }

        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            using var fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite);
            var count = IsZeroByte(fs, _tagBuffer);
            if (count == 0)
            {
                return false;
            }
            FoundChanged?.Invoke(new Models.FileInfoItem(fileInfo));
            var end = fs.Length - count;
            var buffer = new byte[Math.Min(end, 1024)];
            for (var i = 0L; i < end; i += buffer.Length)
            {
                var len = (int)Math.Min(end - i, buffer.Length);
                fs.Seek(i + count, SeekOrigin.Begin);
                fs.Read(buffer, 0, len);
                fs.Seek(i, SeekOrigin.Begin);
                fs.Write(buffer, 0, len);
            }
            fs.SetLength(end);
            return true;
        }

        private int IsZeroByte(Stream stream, byte[] begin)
        {
            var count = 0;
            var offset = -1;
            while (true)
            {
                var b = stream.ReadByte();
                if (b < 0)
                {
                    return 0;
                }
                if (offset >= 0)
                {
                    offset++;
                    if (offset >= begin.Length)
                    {
                        break;
                    }
                    if (begin[offset] != b)
                    {
                        return 0;
                    }
                    continue;
                }
                if (b == 0)
                {
                    count++;
                    continue;
                }
                offset = 0;
                if (begin[offset] != b)
                {
                    return 0;
                }
            }
            return count;
        }
    }
}
