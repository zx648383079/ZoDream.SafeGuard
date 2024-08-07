using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace ZoDream.Shared.Plugins.Compress
{
    public partial class CompressStream
    {
        private long ReadLength()
        {
            var code = stream.ReadByte();
            if (code <= 250)
            {
                return code;
            }
            if (code <= 252)
            {
                return stream.ReadByte() + code * (code - 250);
            }
            var len = code - 251;
            var buffer = new byte[len];
            stream.Read(buffer, 0, len);
            var res = 0L;
            for (var j = len - 2; j >= 0; j--)
            {
                res += (long)Math.Pow(code, j);
            }
            for (var i = 0; i < len; i++)
            {
                res += (long)(buffer[i] * Math.Pow(256, len - i - 1));
            }
            return res;
        }

        private int ReadBytes(byte[] buffer, int length)
        {
            var len = stream.Read(buffer, 0, length);
            if (len == 0)
            {
                return len;
            }
            for (var i = 0; i < len; i++)
            {
                var code = dict.ReadByte();
                buffer[i] = (byte)InflateStream.Clamp(
                    _nextPadding ? (buffer[i] - code) : (buffer[i] + code),
                    256);
            }
            return len;
        }

        public string ReadName()
        {
            if (!_withName)
            {
                return string.Empty;
            }
            var length = ReadLength();
            Debug.WriteLine($"name len: {length}");
            var buffer = new byte[length];
            ReadBytes(buffer, buffer.Length);
            _nextPadding = !_nextPadding;
            return Encoding.UTF8.GetString(buffer);
        }
        private void ReadStream(Stream output)
        {
            var buffer = new byte[_maxBufferLength];
            var length = ReadLength();
            Debug.WriteLine($"len: {length}->{_nextPadding}");
            if (length < 0)
            {
                return;
            }
            var i = 0L;
            while (i < length)
            {
                var len = ReadBytes(buffer, (int)Math.Min(length - i, buffer.Length));
                if (len == 0)
                {
                    return;
                }
                output.Write(buffer, 0, len);
                i += len;
            }
            _nextPadding = !_nextPadding;
            Debug.WriteLine($"pos: {stream.Position}");
        }

        private void JumpPart()
        {
            var len = ReadLength();
            stream.Seek(len, SeekOrigin.Current);
        }

        public IEnumerable<string> ReadFile()
        {
            IsSupport();
            if (!_multiple)
            {
                var fileName = ReadName();
                yield return fileName;
                yield break;
            }
            while (stream.Position < stream.Length) 
            {
                var name = ReadName();
                JumpPart();
                yield return name;
            }
        }

        private string ReadToFile(string folder)
        {
            return ReadToFile(folder, ReadName());
        }

        private string ReadToFile(string folder, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = InflateStream.RandomName(folder);
            }
            var file = Path.Combine(folder, InflateStream.GetSafePath(fileName));
            var dirname = Path.GetDirectoryName(file);
            if (!Directory.Exists(dirname))
            {
                Directory.CreateDirectory(dirname);
            }
            using var fs = File.Create(file);
            ReadStream(fs);
            return fileName;
        }

        public IEnumerable<string> ReadFile(string folder)
        {
            IsSupport();
            if (!_multiple)
            {
                var fileName = ReadToFile(folder);
                yield return fileName;
                yield break;
            }
            while (stream.Position < stream.Length)
            {
                var name = ReadToFile(folder);
                yield return name;
            }
        }

        public IEnumerable<string> ReadFile(string folder, params string[] items)
        {
            IsSupport();
            if (!_multiple)
            {
                var fileName = ReadToFile(folder);
                yield return fileName;
                yield break;
            }
            while (stream.Position < stream.Length)
            {
                var name = ReadName();
                if (!items.Contains(name))
                {
                    JumpPart();
                    continue;
                }
                name = ReadToFile(folder, name);
                yield return name;
            }
        }
    }
}
