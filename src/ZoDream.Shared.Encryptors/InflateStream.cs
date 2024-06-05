using System;
using System.IO;

namespace ZoDream.Shared.Encryptors
{
    public class InflateStream(Stream stream, IEncryptor encryptor) : Stream
    {
        public Stream BaseStream { get; private set; } = stream;

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => BaseStream.CanWrite;

        public override long Length => BaseStream.Length;

        public override long Position 
        { 
            get => BaseStream.Position;
            set => throw new NotSupportedException(); 
        }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(encryptor.Encrypt(buffer), offset, count);
        }
    }
}
