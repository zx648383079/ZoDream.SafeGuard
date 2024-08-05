using SharpCompress.Compressors.Xz;
using System.IO;
using System.Threading;
using ZoDream.Shared.Plugins.Transformers;

namespace ZoDream.Shared.Plugins.Compress
{
    public class InflateTransformer : BaseStreamTransformer, IFinderCompress
    {

        public string OutputFolder { set; private get; } = string.Empty;
        public string DictionaryFileName { set; private get; } = string.Empty;
        public string Password { set; private get; } = string.Empty;

        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            var input = new InflateStream(fileInfo.FullName,
                new CompressDictionary(DictionaryFileName));
            input.TransferTo(OutputFolder);
            EmitFound(fileInfo);
            return true;
        }

        protected override bool IsValidFile(Stream stream, CancellationToken token = default)
        {
            return true;
        }

        protected override void TranformFile(Stream stream, CancellationToken token = default)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var input = new InflateStream(stream, 
                new CompressDictionary(DictionaryFileName));
            input.TransferTo(OutputFolder);
        }
    }
}
