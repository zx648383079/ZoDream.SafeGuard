using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Plugins.Transformers
{
    public abstract class BaseStreamTransformer : StorageFinder, 
        ITransformFinder, IFinderProgress
    {
        public event FinderEventHandler? FoundChanged;

        public event FinderProgressEventHandler? FileProgress;

        private string _lastFile = string.Empty;

        public void Add(IFileTransformer transformer)
        {
        }

        public void Clear()
        {
        }

        public virtual string Transform(string content, CancellationToken token = default)
        {
            return content;
        }

        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            _lastFile = fileInfo.FullName;
            using var fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite);
            if (!IsValidFile(fs, token))
            {
                return false;
            }
            EmitFound(fileInfo);
            TranformFile(fs, token);
            return true;
        }

        protected void EmitFound(FileInfo info)
        {
            FoundChanged?.Invoke(new FileInfoItem(info));
        }
        protected void EmitProgress(long current, long total)
        {
            EmitProgress(_lastFile, current, total);
        }
        protected void EmitProgress(string fileName, long current, long total)
        {
            FileProgress?.Invoke(fileName, current, total);
        }

        protected abstract void TranformFile(Stream stream, CancellationToken token = default);

        protected abstract bool IsValidFile(Stream stream, CancellationToken token = default);
        
        /// <summary>
        /// 判断下一个字符是否是
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected bool NextIs(Stream stream, byte[] buffer)
        {
            if (buffer.Length == 0)
            {
                return false;
            }
            var index = 0;
            var pos = stream.Position;
            while (true)
            {
                var b = stream.ReadByte();
                if (b < 0)
                {
                    index = 0;
                    break;
                }
                if (buffer[index] != b)
                {
                    index = 0;
                    break;
                }
                ++index;
                if (index == buffer.Length)
                {
                    break;
                }
            }
            stream.Seek(pos, SeekOrigin.Begin);
            return index > 0;
        }

        /// <summary>
        /// 无限查找使用这个
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected long IndexOf(Stream stream, byte[] buffer)
        {
            if (buffer.Length == 0)
            {
                return -1;
            }
            var matches = new List<int>();
            var pos = stream.Position;
            while (true)
            {
                var b = stream.ReadByte();
                if (b < 0)
                {
                    break;
                }
                if (buffer[0] == b)
                {
                    matches.Add(-1);
                }
                for (var i = matches.Count - 1; i >= 0; i--)
                {
                    if (buffer[matches[i] + 1] == b)
                    {
                        matches[i]++;
                        if (matches[i] >= buffer.Length - 1)
                        {
                            var res = stream.Position - matches[i];
                            stream.Seek(pos, SeekOrigin.Begin);
                            return res;
                        }
                    } else
                    {
                        matches.RemoveAt(i);
                    }
                }
            }
            stream.Seek(pos, SeekOrigin.Begin);
            return -1;
        }
        /// <summary>
        /// 有限查找请使用这个
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="buffer"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        protected long IndexOf(Stream stream, byte[] buffer, long max)
        {
            if (buffer.Length == 0)
            {
                return -1;
            }
            var position = -1L;
            var matchIndex = 0;
            var originalPosition = stream.Position;
            while (true)
            {
                var nextByte = stream.ReadByte();
                if (nextByte == -1)
                {
                    break;
                }
                if (max >= 0 && stream.Position - originalPosition > max)
                {
                    break;
                }
                if (nextByte == buffer[matchIndex])
                {
                    matchIndex++;
                    if (matchIndex == buffer.Length)
                    {
                        position = stream.Position - buffer.Length;
                        break;
                    }
                }
                else
                {
                    matchIndex = 0;
                }
            }
            stream.Seek(originalPosition, SeekOrigin.Begin);
            return position;
        }

        /// <summary>
        /// 移除字符
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        protected void RemoveByte(Stream stream, long length)
        {
            var end = stream.Length - length;
            var buffer = new byte[Math.Min(end, 1024 * 100)];
            for (var i = stream.Position; i < end; i += buffer.Length)
            {
                EmitProgress(i, end);
                var len = (int)Math.Min(end - i, buffer.Length);
                stream.Seek(i + length, SeekOrigin.Begin);
                stream.Read(buffer, 0, len);
                stream.Seek(i, SeekOrigin.Begin);
                stream.Write(buffer, 0, len);
            }
            stream.SetLength(end);
        }
    }
}
