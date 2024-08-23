using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Plugins.Filters;

namespace ZoDream.Shared.Plugins.Transformers
{
    public class CopyFileTransformer : StorageTransformFinder, IFinderOutput
    {
        public string BaseFolder { get; set; } = string.Empty;

        public string OutputFolder {  get; set; } = string.Empty;

        public bool IsMove { get; set; }

        public ITransformerFilter Filter { get; set; } =
            new NikkiTransformerFilter();
           // new NoneTransformerFilter();


        protected override void OnReady(IEnumerable<string> items)
        {
            if (items.Count() == 1 && string.IsNullOrWhiteSpace(BaseFolder)
                && Directory.Exists(items.First()))
            {
                BaseFolder = items.First();
            }
            Filter.Ready();
        }

        public override string TransformTo(string content)
        {
            return content.Replace(BaseFolder, OutputFolder);
        }


        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(OutputFolder))
            {
                return false;
            }
            TrySetRoot(fileInfo.DirectoryName!);
            return Filter.IsMatchFile(fileInfo, token);
        }

        private void TrySetRoot(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                return;
            }
            if (!string.IsNullOrWhiteSpace(BaseFolder)
                && (folder.StartsWith(BaseFolder) || BaseFolder.StartsWith(folder)))
            {
                return;
            }
            BaseFolder = folder;
            Debug.WriteLine(folder);
        }

        protected override bool IsValidFile(DirectoryInfo folderInfo, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(OutputFolder))
            {
                return false;
            }
            TrySetRoot(folderInfo.Parent!.FullName);
            return false;
        }

        protected override FileInfoItem TranformFile(DirectoryInfo folder, bool isPreview, CancellationToken token)
        {
            var name = GetRelativeFileName(folder.FullName);
            var arg = new MoveFileItem(folder);
            if (string.IsNullOrWhiteSpace(name))
            {
                arg.MoveTo(OutputFolder);
            }
            else
            {
                arg.Name = folder.Name;
                arg.TargetName = Path.Combine(OutputFolder, name);
            }
            if (!isPreview && !string.IsNullOrEmpty(arg.TargetName) && !Directory.Exists(arg.TargetName))
            {
                // Directory.CreateDirectory(arg.TargetName);
            }
            return arg;
        }

        protected override FileInfoItem TranformFile(FileInfo file, bool isPreview, CancellationToken token)
        {
            var name = GetRelativeFileName(file.FullName);
            var arg = new MoveFileItem(file)
            {
                Name = file.Name,
                TargetName = Path.Combine(OutputFolder, 
                    Filter.TranformFileName(name)
                )
            };
            if (!isPreview)
            {
                var folder = Path.GetDirectoryName(arg.TargetName);
                if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                if (IsMove)
                {
                    file.MoveTo(arg.TargetName, true);
                } else
                {
                    file.CopyTo(arg.TargetName, true);
                }
            }
            return arg;
        }

        private string GetRelativeFileName(string fileName) 
        {
            if (fileName == BaseFolder || !fileName.StartsWith(BaseFolder))
            {
                return string.Empty;// Path.GetFileName(file);
            }
            return fileName.Substring(BaseFolder.Length + 1);
        }

    }
}
