using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Plugins.Transformers
{
    public class BatchBackupTransformer : StorageTransformFinder
    {
        public override string TransformTo(string content)
        {
            return string.Join('\n', content.Split('\n').Select(GetArchiveName));
        }

        public string GetArchiveName(string fileName)
        {
            var i = fileName.LastIndexOf('.');
            if (i > 0)
            {
                fileName = fileName[0..i];
            }
            return $"{fileName}.zip";
        }

        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            return fileInfo.Extension.ToLower() == "txt";
        }

        protected override bool IsValidFile(DirectoryInfo folderInfo, CancellationToken token = default)
        {
            return true;
        }

        protected override FileInfoItem TranformFile(DirectoryInfo folder, bool isPreview, CancellationToken token)
        {
            return new FileInfoItem(folder);
        }

        protected override FileInfoItem TranformFile(FileInfo file, bool isPreview, CancellationToken token)
        {
            var arg = new RenameFileItem(file)
            {
                RenameName = GetArchiveName(file.Name)
            };
            if (isPreview)
            {
                return arg;
            }
            Archive(Path.Combine(file.DirectoryName!, arg.RenameName), file.FullName, token);
            return arg;
        }

        private bool Archive(string archiveFileName, string sourceFileName, CancellationToken token)
        {
            return Archive(archiveFileName, File.ReadAllLines(sourceFileName), token);
        }

        private bool Archive(
            string archiveFileName, IEnumerable<string> items, CancellationToken token)
        {
            items = items.Where(item => !string.IsNullOrWhiteSpace(item));
            if (!items.Any())
            {
                return false;
            }
            var transformer = new BackupTransformer(archiveFileName);
            transformer.FileChanged += Notify;
            transformer.Start(items, token);
            return true;
        }

    }
}
