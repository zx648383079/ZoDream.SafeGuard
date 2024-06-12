using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Plugins.Transformers
{
    /// <summary>
    /// Nas安全文件名修改 
    /// </summary>
    public partial class NasRenameTransformer: ITransformFinder, IFinderPreview
    {
        private CancellationTokenSource? _cancelTokenSource;
        public event FinderEventHandler? FoundChanged;
        public event FinderLogEventHandler? FileChanged;
        public event FinderFinishedEventHandler? Finished;

        public bool IsPreview { get; set; }

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
            return TransformTo(content);
        }

        public string TransformTo(string content)
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
            if (!IsValidFile(folder, token))
            {
                EachFiles(folder, token);
                return;
            }
            var (target, name) = TryCreateFolder(folder.Parent!.FullName, TransformTo(folder.Name));
            var arg = new Models.RenameFileItem(folder)
            {
                RenameName = name,
            };
            if (!IsPreview)
            {
                try
                {
                    folder.MoveTo(target);
                    arg.FileName = target;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            FoundChanged?.Invoke(arg);
            EachFiles(new DirectoryInfo(arg.FileName), token);
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
            if (!IsValidFile(file, token))
            {
                return;
            }
            var (target, name) = TryCreateFile(file.DirectoryName!, TransformTo(file.Name));
            var arg = new Models.RenameFileItem(file)
            {
                RenameName = name,
            };
            if (!IsPreview)
            {
                try
                {
                    file.MoveTo(target);
                    arg.FileName = target;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            FoundChanged?.Invoke(arg);
        }

        private (string, string) TryCreateFile(string folder, string name)
        {
            var fileName = Path.Combine(folder, name);
            var extension = string.Empty;
            var rename = name;
            var j = name.IndexOf('.');
            if (j >= 0)
            {
                extension = name[j..];
                name = name[..j];
            }
            var i = 0;
            while (File.Exists(fileName))
            {
                rename = $"{name}_{++i}{extension}";
                fileName = Path.Combine(folder, rename);
            }
            return (fileName, rename);
        }

        private (string, string) TryCreateFolder(string folder, string name)
        {
            var fileName = Path.Combine(folder, name);
            var rename = name;
            var i = 0;
            while (Directory.Exists(fileName))
            {
                rename = $"{name}_{++i}";
                fileName = Path.Combine(folder, rename);
            }
            return (fileName, rename);
        }

        [GeneratedRegex(@"[@#\$\\/'""\|:;\*\?\<\>]+")]
        private static partial Regex UnsafeFileRegex();
    }
}
