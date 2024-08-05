using System.IO;
using System.Threading;
using ZoDream.Shared.Plugins.Transformers;

namespace ZoDream.Shared.Plugins.Compress
{
    public class DeflateTransformer : BaseStreamTransformer, IFinderCompress
    {

        public string OutputFolder { set; private get; } = string.Empty;
        public string DictionaryFileName { set; private get; } = string.Empty;
        public string Password { set; private get; } = string.Empty;

        protected override bool IsValidFile(Stream stream, CancellationToken token = default)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            if (buffer[0] != 0x23 || buffer[1] != 0x5A || buffer[3] != 0x0A)
            {
                return false;
            }
            if (buffer[2] == 0x2)
            {
                return true;
            }
            if (buffer[2] == 0x1)
            {
                return true;
            }
            return false;
        }

        protected override void TranformFile(Stream stream, CancellationToken token = default)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var input = new DeflateStream(stream, new CompressDictionary(DictionaryFileName));
            input.TransferTo(OutputFolder);
        }
    }
}
