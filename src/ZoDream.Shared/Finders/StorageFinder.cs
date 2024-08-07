using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System;

namespace ZoDream.Shared.Finders
{
    public abstract class StorageFinder
    {
        private CancellationTokenSource? _cancelTokenSource;

        public event FinderLogEventHandler? FileChanged;

        public event FinderFinishedEventHandler? Finished;


        public void Start(IEnumerable<string> folders)
        {
            _cancelTokenSource?.Cancel();
            _cancelTokenSource = new CancellationTokenSource();
            var token = _cancelTokenSource.Token;
            Task.Factory.StartNew(() => {
                CheckAnyFile(Preprocess(folders, token), token);
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

        protected virtual IEnumerable<string> Preprocess(IEnumerable<string> files, CancellationToken token = default)
        {
            return files;
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
            EachFiles(fileName, items => {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                foreach (var item in items)
                {
                    CheckFile(item, token);
                }
            }, token);
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

        protected void OnCheckingFile(FileInfo file)
        {
            FileChanged?.Invoke(file.FullName);
        }

        protected virtual void CheckFile(FileInfo file, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            OnCheckingFile(file);
            if (!IsValidFile(file, token))
            {
                return;
            }
            ProcessFile(file, token);
        }

        protected abstract bool IsValidFile(FileInfo fileInfo, CancellationToken token = default);

        protected virtual Task ProcessFile(FileInfo fileInfo, CancellationToken token = default)
        {
            return Task.CompletedTask;
        } 

        public static void EachFiles(string folder, 
            Action<IEnumerable<string>> success,
            CancellationToken token = default)
        {
            try
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }
                Array.ForEach(Directory.GetDirectories(folder), fileName => {
                    EachFiles(fileName, success, token);
                });
                success.Invoke(Directory.GetFiles(folder));
            }
            catch (UnauthorizedAccessException)
            {

            }
        }


        /// <summary>
        /// 获取拓展名，不包含 .
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return extension.Length > 0 ? extension[1..].ToLower() : string.Empty;
        }

        /// <summary>
        /// 获取拓展名，不包含 .
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetExtension(FileInfo file)
        {
            return file.Extension.Length > 0 ? file.Extension[1..].ToLower() : string.Empty;
        }
    }
}
