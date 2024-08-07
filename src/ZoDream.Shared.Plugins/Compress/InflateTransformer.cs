using System.Collections.Generic;
using System.IO;
using System.Threading;
using ZoDream.Shared.Models;
using ZoDream.Shared.Plugins.Transformers;

namespace ZoDream.Shared.Plugins.Compress
{
    public class InflateTransformer : BaseStreamTransformer, IFinderCompress
    {

        public string OutputFolder { set; private get; } = string.Empty;
        public string DictionaryFileName { set; private get; } = string.Empty;
        public string Password { set; private get; } = string.Empty;

        public bool Multiple { private get; set; }

        protected override IEnumerable<string> Preprocess(IEnumerable<string> files, CancellationToken token = default)
        {
            if (!Multiple)
            {
                return base.Preprocess(files, token);
            }
            var outputFile = Path.Combine(OutputFolder, InflateStream.RandomName(OutputFolder));
            using var fs = File.Create(outputFile);
            var output = new CompressStream(
                fs,
                new CompressDictionary(DictionaryFileName));
            output.WriteHeader(true, true);
            foreach (var item in files)
            {
                var fileName = Path.GetFullPath(item);
                if (Directory.Exists(fileName))
                {
                    var baseFolder = Path.GetDirectoryName(fileName);
                    foreach (var it in Directory.GetFiles(fileName, "*", SearchOption.AllDirectories))
                    {
                        EmitProgress(it, 0, 1);
                        output.WriteFile(Path.GetRelativePath(baseFolder!, it), it);
                    }
                } else if (File.Exists(fileName))
                {
                    EmitProgress(fileName, 0, 1);
                    output.WriteFile(Path.GetFileName(fileName), fileName);
                }
            }
            fs.Flush();
            EmitFound(new FileInfoItem(outputFile));
            return [];
        }

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
