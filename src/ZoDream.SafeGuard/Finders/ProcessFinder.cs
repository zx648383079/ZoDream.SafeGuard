using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ZoDream.SafeGuard.Plugins;

namespace ZoDream.SafeGuard.Finders
{
    public class ProcessFinder: StorageFinder
    {
        public IList<IFileProcess>? ProcessItems { get; set; } = [];

        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            if (ProcessItems == null || ProcessItems.Count == 0)
            {
                return true;
            }
            var extension = fileInfo.Extension[1..].ToLower();
            using var fileLoader = new FileLoader(fileInfo);
            var isMatch = false;
            foreach (var process in ProcessItems)
            {
                if (!process.LoadExtension().Contains(extension))
                {
                    continue;
                }
                isMatch = true;
                foreach (var item in process.LoadFilters())
                {
                    if (token.IsCancellationRequested)
                    {
                        return false;
                    }
                    if (!item.Valid(fileLoader, token))
                    {
                        return false;
                    }
                }
            }
            return isMatch;
        }
    }
}
