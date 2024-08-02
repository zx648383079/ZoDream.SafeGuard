using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ZoDream.Shared.Plugins.Compress
{
    /// <summary>
    /// 加密流
    /// </summary>
    public class InflateStream: IDisposable
    {

        public InflateStream(string fileName, bool withName = true)
            : this(File.OpenRead(fileName))
        {
            if (withName)
            {
                WriteName(Path.GetFileName(fileName));
            }
        }

        public InflateStream(Stream input)
        {
            BaseStream = input;
        }

        private string _name = string.Empty;
        private bool _nextPadding = false;
        public bool WithName => !string.IsNullOrWhiteSpace(_name);

        public Stream BaseStream { get; }

        private void WriteHeader(Stream output)
        {
            output.WriteByte(0x23);
            output.WriteByte(0x5A);
            output.WriteByte((byte)(WithName ? 1 : 2));
            output.WriteByte(0x0A);
        }

        private void WriteName(Stream output)
        {
            if (!WithName)
            {
                return;
            }
            WriteBytes(output, Encoding.UTF8.GetBytes(_name));
        }

        private void WriteLength(Stream output, long length)
        {
            if (length <= 250)
            {
                output.WriteByte((byte)length);
                return;
            }
            byte i;
            var basic = 250;
            // 相加
            for (i = 251; i <= 252; i++)
            {
                var plus = i * (i - basic);
                if (length <= plus + 255)
                {
                    output.WriteByte(i);
                    output.WriteByte((byte)(length - plus));
                    return;
                }
            }
            // 倍数
            basic = 252;
            for (i = 253; i <= 255; i++)
            {
                var len = i - basic + 1;
                var buffer = new byte[len + 1];
                buffer[len] = i;
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
                    output.Write(buffer.Reverse().ToArray());
                }
            }
            
        }

        private void WriteBytes(Stream output, byte[] buffer)
        {
            WriteBytes(output, buffer, buffer.Length);
        }

        private void WriteBytes(Stream output, byte[] buffer, int max)
        {
            var begin = 0;
            for (int i = 0; i < max; i++)
            {
                if (!TryPadding(buffer[i], out var res))
                {
                    buffer[i] = res;
                    continue;
                }
                var len = i - begin;
                WriteLength(output, len);
                if (len > 0)
                {
                    output.Write(buffer, begin, len);
                }
                buffer[i] = res;
                begin = i;
            }
            if (begin < max)
            {
                var len = max - begin;
                WriteLength(output, len);
                output.Write(buffer, begin, len);
            }
        }

        private void WriteStream(Stream output, Stream input)
        {
            var buffer = new byte[4096];
            while (true)
            {
                var len = input.Read(buffer, 0, buffer.Length);
                if (len == 0)
                {
                    break;
                }
                WriteBytes(output, buffer, len);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns>是否切换新的规则</returns>
        private bool TryPadding(byte input, out byte output)
        {
            var code = ReadDict();
            var b = _nextPadding ? (input + code) : (input - code);
            if (b >= 0 && b <= 255)
            {
                output = (byte)b;
                return false;
            }
            _nextPadding = !_nextPadding;
            output = _nextPadding ? (byte)(input + code) : (byte)(input - code);
            return true;
        }

        private byte ReadDict()
        {
            return 0x0;
        }

        public void WriteName(string name)
        {
            _name = name.Trim();
        }

        public void TransferTo(Stream output)
        {
            _nextPadding = false;
            WriteHeader(output);
            WriteName(output);
            WriteStream(output, BaseStream);
        }

        public void TransferTo(string folder)
        {
            var name = "z_" + MD5Encode(folder + DateTime.Now.ToLongTimeString());
            using var fs = File.Create(Path.Combine(folder, name));
            TransferTo(fs);
        }

        public static string MD5Encode(string source)
        {
            var sor = Encoding.UTF8.GetBytes(source);
            var result = MD5.HashData(sor);
            return Convert.ToHexString(result).ToLower();
        }

        public void Dispose()
        {
            BaseStream.Dispose();
        }
    }
}
