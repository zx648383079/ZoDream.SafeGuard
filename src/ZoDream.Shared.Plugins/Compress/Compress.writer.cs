using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ZoDream.Shared.Plugins.Compress
{
    public partial class CompressStream
    {
        public void WriteHeader(bool withName, bool multiple)
        {
            _withName = withName || multiple;
            _multiple = multiple;
            stream.WriteByte(0x23);
            stream.WriteByte(0x5A);
            if (multiple)
            {
                stream.WriteByte(3);
            }
            else
            {
                stream.WriteByte((byte)(withName ? 1 : 2));
            }
            stream.WriteByte(0x0A);
        }

        public void WriteFile(string fileName, Stream input) 
        {
            if (_withName)
            {
                WriteName(fileName);
            }
            WriteStream(input);
        }

        public void WriteFile(string fileName, string FullName)
        {
            using var fs = File.OpenRead(FullName);
            WriteFile(fileName, fs);
        }

        private void WriteName(string fileName)
        {
            var buffer = Encoding.UTF8.GetBytes(fileName);
            Debug.WriteLine($"name len: {buffer.Length}");
            WriteLength(buffer.Length);
            WriteBytes(buffer, buffer.Length);
            _nextPadding = !_nextPadding;
        }

        private void WriteLength(long length)
        {
            if (length <= 250)
            {
                stream.WriteByte((byte)length);
                return;
            }
            var i = 0;
            var basic = 250;
            // 相加
            for (i = 251; i <= 252; i++)
            {
                var plus = i * (i - basic);
                if (length <= plus + 255)
                {
                    stream.WriteByte((byte)i);
                    stream.WriteByte((byte)(length - plus));
                    return;
                }
            }
            // 倍数
            basic = 252;
            i = 253;
            for (; i <= 255; i++)
            {
                var len = i - basic + 1;
                var buffer = new byte[len + 1];
                buffer[len] = (byte)i;
                var b = 0L;
                for (var j = len - 2; j >= 0; j--)
                {
                    b += (long)Math.Pow(i, j);
                }
                var rate = length - b;
                for (var j = 0; j < len; j++)
                {
                    buffer[j] = (byte)(rate % 256);
                    rate /= 256;
                }
                if (rate == 0)
                {
                    stream.Write(buffer.Reverse().ToArray());
                    break;
                }
            }

        }

        private void WriteBytes(byte[] buffer, int max)
        {
            for (int i = 0; i < max; i++)
            {
                var code = dict.ReadByte();
                buffer[i] = (byte)InflateStream.Clamp(
                    _nextPadding ? (buffer[i] + code) : (buffer[i] - code)
                    , 256);
            }
            stream.Write(buffer, 0, max);
        }

        private void WriteStream(Stream input)
        {
            var buffer = new byte[_maxBufferLength];
            var length = input.Length - input.Position;
            Debug.WriteLine($"len: {length}->{_nextPadding}"); ;
            WriteLength(length);
            while (true)
            {
                var len = input.Read(buffer, 0, buffer.Length);
                if (len == 0)
                {
                    break;
                }
                WriteBytes(buffer, len);
            }
            Debug.WriteLine($"pos: {stream.Position}");
            _nextPadding = !_nextPadding;
        }
    }
}
