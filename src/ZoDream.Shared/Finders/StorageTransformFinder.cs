using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Finders
{
    /// <summary>
    /// 只对文件进行遍历
    /// </summary>
    public abstract class StorageTransformFinder : ITransformFinder, IFinderPreview
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
                OnReady(folders);
                CheckAnyFile(folders, token);
                OnFinished(token);
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


        protected virtual void OnFinished(CancellationToken token)
        {

        }

        protected void Notify(string message) 
        {
            FileChanged?.Invoke(message);
        }

        public string Transform(string content, CancellationToken token = default)
        {
            return TransformTo(content);
        }

        protected virtual void OnReady(IEnumerable<string> items)
        {

        }

        /// <summary>
        /// 转化测试文本
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public abstract string TransformTo(string content);
        /// <summary>
        /// 是否是需要变更的文件
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected abstract bool IsValidFile(FileInfo fileInfo, CancellationToken token = default);
        /// <summary>
        /// 是否是需要变更的文件夹
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected abstract bool IsValidFile(DirectoryInfo folderInfo, CancellationToken token = default);

        /// <summary>
        /// 生成新的文件路径
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="isPreview"></param>
        /// <returns></returns>
        protected abstract FileInfoItem TranformFile(DirectoryInfo folder, bool isPreview, CancellationToken token);
        /// <summary>
        /// 生成新的文件路径
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="isPreview"></param>
        /// <returns></returns>
        protected abstract FileInfoItem TranformFile(FileInfo file, bool isPreview, CancellationToken token);


        protected void CheckAnyFile(IEnumerable<string> folders, CancellationToken token = default)
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
            var arg = TranformFile(folder, IsPreview, token);
            FoundChanged?.Invoke(arg);
            EachFiles(new DirectoryInfo(arg.FileName), token);
        }

        private void EachFiles(DirectoryInfo folder, CancellationToken token = default)
        {
            try
            {
                if (!folder.Exists)
                {
                    return;
                }
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
            var arg = TranformFile(file, IsPreview, token);
            FoundChanged?.Invoke(arg);
        }

        /// <summary>
        /// 尝试创建新文件，存在自动生成新的
        /// </summary>
        /// <param name="folder">文件夹路径</param>
        /// <param name="name"></param>
        /// <returns>完整文件路径，文件名</returns>
        public static (string, string) TryCreateFile(string folder, string name)
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

        /// <summary>
        /// 尝试创建新文件夹，存在自动生成新的
        /// </summary>
        /// <param name="folder">父文件夹路径</param>
        /// <param name="name"></param>
        /// <returns>完整文件路径，文件名</returns>
        public static (string, string) TryCreateFolder(string folder, string name)
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
    }
}
