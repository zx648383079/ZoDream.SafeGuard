using SharpCompress.Compressors.Lzw;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ZoDream.Shared.Plugins.Transformers
{
    public class TarRepairTransformer : BaseStreamTransformer
    {

        protected override bool IsValidFile(Stream stream, CancellationToken token = default)
        {
            return true;
            stream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[2];
            stream.Read(buffer, 0, 2);
            return buffer[0] == 0xFF && buffer[1] == 0xFE;
        }

        protected override void TranformFile(Stream stream, CancellationToken token = default)
        {
            //if (!IsZero(stream))
            //{
            //    RemoveByte(stream, 2);
            //    return;
            //}
            stream.Seek(0, SeekOrigin.Begin);
            var jump = 0;
            var zero = 0L;
            var end = stream.Length - jump;
            var buffer = new byte[Math.Min(end, 1024 * 100)];
            var i = stream.Position;
            while (i < end)
            {
                EmitProgress(i, end);
                var len = (int)Math.Min(end - i, buffer.Length);
                stream.Seek(i + jump, SeekOrigin.Begin);
                len = stream.Read(buffer, 0, len);
                var z = 0;
                for (var j = 0; j < len; j++)
                {
                    if ((i + j) % 2 == 1 && buffer[j] == 0x0)
                    {
                        z++;
                    } else if (z > 0)
                    {
                        buffer[j - z] = buffer[j];
                    }
                }
                stream.Seek(i - zero, SeekOrigin.Begin);
                stream.Write(buffer, 0, len - z);
                zero += z;
                i += len;
            }
            stream.SetLength(end - zero);
            stream.Flush();
        }

        private bool IsZero(Stream stream)
        {
            stream.Seek(3, SeekOrigin.Begin);
            var i = 2;
            while (i < 50)
            {
                var b = stream.ReadByte();
                if (b < 0)
                {
                    break;
                }
                i++;
                if (i % 2 == 1 && b != 0x0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
