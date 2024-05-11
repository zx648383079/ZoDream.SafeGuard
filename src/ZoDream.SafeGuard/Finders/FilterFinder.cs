using System.Collections.Generic;
using System.IO;
using System.Threading;
using ZoDream.SafeGuard.Plugins;

namespace ZoDream.SafeGuard.Finders
{
    public class FilterFinder: StorageFinder
    {
        public IList<IFileFilter>? FilterItems { get; set; } = [];

        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            if (FilterItems == null || FilterItems.Count == 0)
            {
                return true;
            }
            using var fileLoader = new FileLoader(fileInfo);
            foreach (var item in FilterItems)
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
            return true;
        }
    }
}
