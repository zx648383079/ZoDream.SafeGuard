using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Plugins.Compress
{
    public class DictionaryTransformer : ITransformFinder
    {
        private CancellationTokenSource? _cancelTokenSource;
        public event FinderLogEventHandler? FileChanged;
        public event FinderEventHandler? FoundChanged;
        public event FinderFinishedEventHandler? Finished;

        public string OutputFileName { private get; set; } = string.Empty;

        public void Add(IFileTransformer transformer)
        {
        }

        public void Clear()
        {
        }

        public void Start(IEnumerable<string> folders)
        {
            _cancelTokenSource?.Cancel();
            _cancelTokenSource = new CancellationTokenSource();
            var token = _cancelTokenSource.Token;
            Task.Factory.StartNew(() => {
                Convert(folders, token);
                Finished?.Invoke();
            }, token);
        }

        public void Stop()
        {
            if (_cancelTokenSource != null)
            {
                _cancelTokenSource.Cancel();
                Finished?.Invoke();
            }
        }

        public string Transform(string content, CancellationToken token = default)
        {
            return content;
        }

        protected void Convert(IEnumerable<string> items, CancellationToken token = default)
        {
            var first = items.FirstOrDefault();
            if (items.Count() == 1 && Directory.Exists(first))
            {
                items = Directory.GetFiles(first);
            }
            var fileItems = items.Where(File.Exists).OrderBy(item => Path.GetFileNameWithoutExtension(item)).ToArray();
            if (fileItems.Length == 0) 
            {
                return;
            }
            var binFile = string.IsNullOrWhiteSpace(OutputFileName) ? Path.Combine(Path.GetDirectoryName(
                Path.GetFullPath(first)), "dict.bin") : OutputFileName;
            Convert(binFile, fileItems, token);
        }

        protected void Convert(string binFile, IEnumerable<string> fileItems, CancellationToken token = default)
        {
            using var output = new FileStream(binFile, FileMode.Create);
            // output.WriteByte(3);
            var buffer = new byte[1024];
            foreach (var item in fileItems)
            {
                if (!File.Exists(item) || token.IsCancellationRequested)
                {
                    continue;
                }
                FileChanged?.Invoke(item);
                using var input = File.OpenRead(item);
                while (true)
                {
                    var count = input.Read(buffer, 0, buffer.Length);
                    if (count == 0)
                    {
                        break;
                    }
                    var j = 0;
                    for (var i = 0; i < count; i+=2)
                    {
                        j = i / 2;
                        buffer[j] = (byte)((buffer[i] - 48) * 10 +
                            (i + 1 >= count ? 0 : (buffer[i + 1] - 48)));
                    }
                    output.Write(buffer, 0, j);
                }
            }
            FoundChanged?.Invoke(new Models.FileInfoItem(binFile));
        }
    }
}
