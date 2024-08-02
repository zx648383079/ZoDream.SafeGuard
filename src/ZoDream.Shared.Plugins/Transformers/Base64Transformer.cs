using System;
using System.IO;
using System.Text;
using System.Threading;

namespace ZoDream.Shared.Plugins.Transformers
{
    public class Base64Transformer : BaseStreamTransformer
    {
        public override string Transform(string content, CancellationToken token = default)
        {
            var bytes = Convert.FromBase64String(content);
            return Encoding.UTF8.GetString(bytes);
        }
        protected override bool IsValidFile(Stream stream, CancellationToken token = default)
        {
            return true;
        }

        protected override void TranformFile(Stream stream, CancellationToken token = default)
        {
            var reader = new StreamReader(stream);
            var data = reader.ReadToEnd();
            var buffer = Convert.FromBase64String(data);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(buffer, 0, buffer.Length);
            stream.SetLength(buffer.Length);
        }
    }
}
