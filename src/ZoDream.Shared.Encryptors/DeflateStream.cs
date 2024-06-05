using System;
using System.IO;

namespace ZoDream.Shared.Encryptors
{
    internal class DeflateStream(Stream stream, IEncryptor encryptor) : Stream
    {

        public Stream BaseStream { get; private set; } = stream;

        public override bool CanRead => BaseStream.CanRead;

        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => false;

        public override long Length => BaseStream.Length;

        public override long Position { get => BaseStream.Position; set => BaseStream.Position = value; }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var len = BaseStream.Read(buffer, offset, count);
            encryptor.Decrypt(buffer);
            return len;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
