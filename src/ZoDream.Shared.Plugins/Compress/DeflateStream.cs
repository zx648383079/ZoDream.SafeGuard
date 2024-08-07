using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ZoDream.Shared.Plugins.Compress
{

    /// <summary>
    /// 解密流
    /// </summary>
    public class DeflateStream
    {
        public DeflateStream(string fileName, CompressDictionary dict)
            : this(File.OpenRead(fileName), dict)
        {
            
        }

        public DeflateStream(Stream input, CompressDictionary dict)
        {
            BaseStream = input;
            _dict = dict;
            ReadHeader();
        }
        private bool _nextPadding = false;
        private long _offset = 0;
        public bool IsMultiple = false;
        private readonly CompressDictionary _dict;

        public string FileName { get; private set; } = string.Empty;
        public Stream BaseStream { get; }

        private void ReadHeader()
        {
            var buffer = new byte[4];
            BaseStream.Read(buffer, 0, 4);
            if (buffer[0] != 0x23 || buffer[1] != 0x5A || buffer[3] != 0x0A)
            {
                throw new FileLoadException("file unsupport");
            }
            if (buffer[2] == 0x2)
            {
                _offset = BaseStream.Position;
                return;
            }
            if (buffer[2] == 0x1)
            {
                FileName = ReadName();
                _offset = BaseStream.Position;
                return;
            }
            if (buffer[3] == 0x3)
            {
                IsMultiple = true;
                return;
            }
            throw new FileLoadException("file unsupport");
        }

        private string ReadName()
        {
            var len = ReadLength();
            Debug.WriteLine($"<-len: {len}");
            var buffer = new byte[len];
            BaseStream.Read(buffer, 0, buffer.Length);
            Restore(buffer, buffer.Length);
            _nextPadding = !_nextPadding;
            return Encoding.UTF8.GetString(buffer);
        }

        private void Restore(byte[] buffer, int length)
        {
            for (var i = 0; i < length; i++)
            {
                TryPadding(buffer[i], out buffer[i]);
            }
        }

        private void ReadStream(Stream output)
        {
            var buffer = new byte[4096];
            while (true)
            {
                var length = ReadLength();
                if (length < 0)
                {
                    return;
                }
                Debug.WriteLine($"<-len: {length}");
                var i = 0L;
                while (i < length)
                {
                    var len = BaseStream.Read(buffer, 0, (int)Math.Min(length - i, buffer.Length));
                    if (len == 0)
                    {
                        return;
                    }
                    Restore(buffer, len);
                    output.Write(buffer, 0, len);
                    i += len;
                }
                _nextPadding = !_nextPadding;
            }
        }

        private long ReadLength()
        {
            var code = BaseStream.ReadByte();
            if (code <= 250)
            {
                return code;
            }
            if (code <= 252)
            {
                return BaseStream.ReadByte() + code * (code - 250);
            }
            var len = code - 251;
            var buffer = new byte[len];
            BaseStream.Read(buffer, 0, len);
            Debug.WriteLine($"[{string.Join(',', buffer)}]");
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

        private byte ReadDict()
        {
            return _dict.ReadByte();
        }

        private bool TryPadding(byte input, out byte output)
        {
            var code = ReadDict();
            var res = _nextPadding ? (input - code) : (input + code);
            output = (byte)InflateStream.Clamp(res, 256);
            return true;
        }


        public void TransferTo(Stream output)
        {
            _nextPadding = !string.IsNullOrWhiteSpace(FileName);
            BaseStream.Seek(_offset, SeekOrigin.Begin);
            ReadStream(output);
        }

        public void TransferTo(string folder)
        {
            var name = FileName;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = InflateStream.RandomName(folder);
            }
            using var fs = File.Create(Path.Combine(folder, InflateStream.GetSafePath(name)));
            TransferTo(fs);
        }
    }
}
