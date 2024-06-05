using System.Text.RegularExpressions;
using ZoDream.Shared.Finders;

namespace ZoDream.Shared.Plugins.Transformers
{
    /// <summary>
    /// Nas安全文件名修改 
    /// </summary>
    public partial class NasRenameTransformer: ITransformFinder
    {
        private CancellationTokenSource? _cancelTokenSource;
        public event FinderEventHandler? FoundChanged;
        public event FinderLogEventHandler? FileChanged;
        public event FinderFinishedEventHandler? Finished;

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
                CheckAnyFile(folders, token);
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
            return Transform(content);
        }

        public string Transform(string content)
        {
            content = UnsafeFileRegex().Replace(content, "");
            return string.IsNullOrWhiteSpace(content) ? "undefined" : content;
        }

        protected bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            return UnsafeFileRegex().IsMatch(fileInfo.Name);
        }

        protected bool IsValidFile(DirectoryInfo fileInfo, CancellationToken token = default)
        {
            return UnsafeFileRegex().IsMatch(fileInfo.Name);
        }


        private void CheckAnyFile(IEnumerable<string> folders, CancellationToken token = default)
        {
            foreach (var item in folders)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                CheckFolderOrFile(item, token);
            }
        }

        private void CheckFolderOrFile(string fileName, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            var file = new FileInfo(fileName);
            if (file.Exists)
            {
                CheckFile(file, token);
                return;
            }
            var folder = new DirectoryInfo(fileName);
            CheckFolder(folder, token);
        }

        private void CheckFolder(DirectoryInfo folder, CancellationToken token)
        {
            FileChanged?.Invoke(folder.FullName);
            if (!IsValidFile(folder))
            {
                EachFiles(folder, token);
                return;
            }
            var target = Path.Combine(folder.Parent!.FullName, Transform(folder.Name));
            folder.MoveTo(target);
            EachFiles(new DirectoryInfo(target), token);
        }

        private void EachFiles(DirectoryInfo folder, CancellationToken token = default)
        {
            try
            {
                foreach (var item in folder.EnumerateDirectories())
                {
                    CheckFolder(item, token);
                }
                foreach (var item in folder.EnumerateFiles())
                {
                    CheckFile(item, token);
                }
            }
            catch (UnauthorizedAccessException)
            {

            }
        }

        private void CheckFile(string fileName, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            var file = new FileInfo(fileName);
            if (!file.Exists)
            {
                return;
            }
            CheckFile(file, token);
        }

        protected virtual void CheckFile(FileInfo file, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            FileChanged?.Invoke(file.FullName);
            if (!IsValidFile(file))
            {
                return;
            }
            FoundChanged?.Invoke(file);
            file.MoveTo(Path.Combine(file.DirectoryName!, Transform(file.Name)));
        }

        [GeneratedRegex(@"[@#\$\\/'""\|:;\*\?\<\>]+")]
        private static partial Regex UnsafeFileRegex();
    }
}
