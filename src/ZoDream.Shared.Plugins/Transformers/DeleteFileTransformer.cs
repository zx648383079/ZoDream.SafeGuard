using System.Collections.Generic;
using System.IO;
using System.Threading;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Plugins.Transformers
{
    /// <summary>
    /// 删除文件及删除空文件夹
    /// </summary>
    public class DeleteFileTransformer : StorageTransformFinder
    {
        private List<string> _items = [];

        protected override void OnReady(IEnumerable<string> items)
        {
            _items.Clear();
        }

        protected override void OnFinished(CancellationToken token)
        {
            var i = 0;
            while (i < _items.Count)
            {
                var fileName = _items[i];
                if (IsEmpty(fileName))
                {
                    Notify(fileName);
                    Directory.Delete(fileName, true);
                    Add(Path.GetDirectoryName(fileName));
                    Thread.Sleep(100);
                }
                i++;
            }
            Notify("Finished");
        }

        private bool IsEmpty(string folder)
        {
            return Directory.GetFiles(folder).Length == 0 && Directory.GetDirectories(folder).Length == 0;
        }

        private bool IsEmpty(DirectoryInfo folder)
        {
            return folder.GetFiles().Length == 0 && folder.GetDirectories().Length == 0;
        }

        public override string TransformTo(string content)
        {
            return string.Empty;
        }

        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            if (fileInfo.Extension == ".cs" || fileInfo.Name.EndsWith(".cs.meta"))
            {
                return true;
            }
            return false;
        }

        protected override bool IsValidFile(DirectoryInfo folderInfo, CancellationToken token = default)
        {
            return IsEmpty(folderInfo);
        }

        protected override FileInfoItem TranformFile(DirectoryInfo folder, bool isPreview, CancellationToken token)
        {
            if (!isPreview)
            {
                folder.Delete();
                Add(folder.Parent?.FullName);
            }
            return new FileInfoItem(folder);
        }

        protected override FileInfoItem TranformFile(FileInfo file, bool isPreview, CancellationToken token)
        {
            if (!isPreview)
            {
                file.Delete();
                Add(file.DirectoryName);
            }
            return new FileInfoItem(file);
        }

        private void Add(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }
            if (_items.Contains(fileName))
            {
                return;           
            }
            var i = fileName.IndexOf(Path.PathSeparator);
            if (i < 0 || i == fileName.Length - 1)
            {
                return;
            }
            _items.Add(fileName);
        }
    }
}
