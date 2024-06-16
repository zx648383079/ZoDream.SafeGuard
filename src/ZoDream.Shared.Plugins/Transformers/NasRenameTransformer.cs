using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Plugins.Transformers
{
    /// <summary>
    /// Nas安全文件名修改 
    /// </summary>
    public partial class NasRenameTransformer: StorageTransformFinder
    {

        public override string TransformTo(string content)
        {
            content = UnsafeFileRegex().Replace(content, "");
            return string.IsNullOrWhiteSpace(content) ? "undefined" : content;
        }

        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            return UnsafeFileRegex().IsMatch(fileInfo.Name);
        }

        protected override bool IsValidFile(DirectoryInfo fileInfo, CancellationToken token = default)
        {
            return UnsafeFileRegex().IsMatch(fileInfo.Name);
        }

        protected override FileInfoItem TranformFile(DirectoryInfo folder, bool isPreview, CancellationToken token)
        {
            var (target, name) = TryCreateFolder(folder.Parent!.FullName, TransformTo(folder.Name));
            var arg = new RenameFileItem(folder)
            {
                RenameName = name,
            };
            if (!isPreview)
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
            return arg;
        }
        protected override FileInfoItem TranformFile(FileInfo file, bool isPreview, CancellationToken token)
        {
            var (target, name) = TryCreateFile(file.DirectoryName!, TransformTo(file.Name));
            var arg = new RenameFileItem(file)
            {
                RenameName = name,
            };
            if (!isPreview)
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
            return arg;
        }



        [GeneratedRegex(@"[@#\$\\/'""\|:;\*\?\<\>]+")]
        private static partial Regex UnsafeFileRegex();
    }
}
