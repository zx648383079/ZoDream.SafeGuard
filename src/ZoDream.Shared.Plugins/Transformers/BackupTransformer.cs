using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Plugins.Transformers
{
    public class BackupTransformer : StorageTransformFinder
    {
        public BackupTransformer()
        {
            
        }

        public BackupTransformer(string fileName)
        {
            TargetFileName = fileName;
        }

        private readonly Dictionary<string, Ignore.Ignore> _filterItems = [];
        private string _baseFolder = string.Empty;
        private readonly Dictionary<string, string> _entityItems = [];
        public string TargetFileName { get; set; } = string.Empty;

        public void Start(IEnumerable<string> items, CancellationToken token)
        {
            _entityItems.Clear();
            CheckAnyFile(items, token);
            OnFinished(token);
        }
        /// <summary>
        /// 忽略一些默认的软件生成文件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        protected virtual bool IsIgnoreDefaultName(string name)
        {
            return name switch
            {
                ".git" or ".idea" or ".vscode" => true,
                _ => false
            };
        }

        private void AddEntry(string name, string fileName)
        {
            _entityItems.TryAdd(name, fileName);
        }

        protected override void OnFinished(CancellationToken token)
        {
            if (_entityItems.Count == 0 || token.IsCancellationRequested)
            {
                return;
            }
            using var archive = ZipArchive.Create();
            foreach (var item in _entityItems)
            {
                archive.AddEntry(item.Key, File.OpenRead(item.Value));
            }
            using var writer = File.Create(TargetFileName);
            archive.SaveTo(writer, CompressionType.None);
        }

        public override string TransformTo(string content)
        {
            return string.Empty;
        }

        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            var splitTag = fileInfo.FullName.Contains('/') ? '/' : '\\';
            foreach (var item in _filterItems)
            {
                if (fileInfo.FullName.StartsWith(item.Key + splitTag) && 
                    item.Value.IsIgnored(fileInfo.FullName))
                {
                    return false;
                }              
            }
            return true;
        }

        protected override bool IsValidFile(DirectoryInfo folderInfo, CancellationToken token = default)
        {
            if (IsIgnoreDefaultName(folderInfo.Name))
            {
                return false;
            }
            var splitTag = folderInfo.FullName.Contains('/') ? '/' : '\\';
            foreach (var item in _filterItems)
            {
                if ((item.Key == folderInfo.FullName || folderInfo.FullName.StartsWith(item.Key + splitTag)) &&
                    item.Value.IsIgnored(folderInfo.FullName))
                {
                    return false;
                }
            }
            return true;
        }

        protected override FileInfoItem TranformFile(DirectoryInfo folder, bool isPreview, CancellationToken token)
        {
            if (string.IsNullOrEmpty(_baseFolder) || 
                !folder.FullName.StartsWith(_baseFolder))
            {
                _baseFolder = folder.Parent!.FullName;
            }
            // var name = Path.GetRelativePath(_baseFolder, folder.FullName);
            // TODO
            var rule = LoadIgnore(folder.FullName);
            if (rule is not null)
            {
                _filterItems.Add(folder.FullName, rule);
            }
            return new FileInfoItem(folder);
        }

        protected override FileInfoItem TranformFile(FileInfo file, bool isPreview, CancellationToken token)
        {
            if (string.IsNullOrEmpty(_baseFolder) ||
                !file.FullName.StartsWith(_baseFolder))
            {
                _baseFolder = file.DirectoryName!;
            }
            var name = Path.GetRelativePath(_baseFolder, file.FullName);
            AddEntry(name, file.FullName);
            return new FileInfoItem(file);
        }

        private Ignore.Ignore? LoadIgnore(string folder)
        {
            var fileName = Path.Combine(folder, ".gitignore");
            if (!File.Exists(fileName))
            {
                return null;
            }
            var items = File.ReadAllLines(fileName)
                .Where(item => !string.IsNullOrWhiteSpace(item) && !item.StartsWith('#'));
            if (!items.Any()) 
            {
                return null;
            }
            return new Ignore.Ignore().Add(items);
        }
    }
}
