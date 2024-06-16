using System.IO;
using System.Threading;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Plugins.Transformers
{
    public class CopyFileTransformer : StorageTransformFinder
    {
        public string BaseFolder { get; set; } = string.Empty;

        public string TargetFolder {  get; set; } = string.Empty;

        public override string TransformTo(string content)
        {
            return content.Replace(BaseFolder, TargetFolder);
        }

        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(TargetFolder))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(BaseFolder))
            {
                BaseFolder = fileInfo.DirectoryName!;
            }
            return true;
        }

        protected override bool IsValidFile(DirectoryInfo folderInfo, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(TargetFolder))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(BaseFolder))
            {
                BaseFolder = folderInfo.FullName;
            }
            return true;
        }

        protected override FileInfoItem TranformFile(DirectoryInfo folder, bool isPreview, CancellationToken token)
        {
            var name = GetRelativeFileName(folder.FullName);
            var arg = new MoveFileItem(folder);
            if (string.IsNullOrWhiteSpace(name))
            {
                arg.MoveTo(TargetFolder);
            }
            else
            {
                arg.Name = folder.Name;
                arg.TargetName = Path.Combine(TargetFolder, name);
            }
            if (!isPreview && !string.IsNullOrEmpty(arg.TargetName) && !Directory.Exists(arg.TargetName))
            {
                Directory.CreateDirectory(arg.TargetName);
            }
            return arg;
        }

        protected override FileInfoItem TranformFile(FileInfo file, bool isPreview, CancellationToken token)
        {
            var name = GetRelativeFileName(file.FullName);
            var arg = new MoveFileItem(file)
            {
                Name = file.Name,
                TargetName = Path.Combine(TargetFolder, name)
            };
            if (!isPreview)
            {
                var folder = Path.GetDirectoryName(arg.TargetName);
                if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                file.CopyTo(arg.TargetName, true);
            }
            return arg;
        }

        private string GetRelativeFileName(string fileName) 
        {
            if (!fileName.StartsWith(BaseFolder))
            {
                return string.Empty;// Path.GetFileName(file);
            }
            return fileName.Substring(BaseFolder.Length + 1);
        }

    }
}
