using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZoDream.Shared.Plugins
{
    public class StreamByteFinder: ByteFinder
    {
        public StreamByteFinder(Stream reader)
        {
            reader.Seek(0, SeekOrigin.Begin);
            _reader = reader;
        }

        private readonly Stream _reader;

        protected override long VerifyLength => _reader.Length;

        protected override byte GetByte(long index)
        {
            _reader.Seek(index, SeekOrigin.Begin);
            return (byte)_reader.ReadByte();
        }

        protected override bool IsInvalidByte(byte code)
        {
            _reader.Seek(0, SeekOrigin.Begin);
            while (true)
            {
                var b = _reader.ReadByte();
                if (b < 0)
                {
                    break;
                }
                if (b == code) {
                    return false;
                }
            }
            return true;
        }
    }
}
