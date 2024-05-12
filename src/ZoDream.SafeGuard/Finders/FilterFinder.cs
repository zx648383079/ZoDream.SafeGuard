using System.Collections.Generic;
using System.IO;
using System.Threading;
using ZoDream.SafeGuard.Models;
using ZoDream.SafeGuard.Plugins;

namespace ZoDream.SafeGuard.Finders
{
    public class FilterFinder: BaseFilterFinder
    {
        public IList<IFileFilter>? FilterItems { get; set; } = [];

        protected override FileCheckStatus CheckFileStatus(FileInfo fileInfo, CancellationToken token = default)
        {
            if (FilterItems == null || FilterItems.Count == 0)
            {
                return FileCheckStatus.Pass;
            }
            using var fileLoader = new FileLoader(fileInfo);
            foreach (var item in FilterItems)
            {
                if (token.IsCancellationRequested)
                {
                    return FileCheckStatus.Pass;
                }
                var status = item.Valid(fileLoader, token);
                if (status == FileCheckStatus.Pass)
                {
                    continue;
                }
                if (status > FileCheckStatus.Normal)
                {
                    return status;
                }
            }
            return FileCheckStatus.Normal;
        }
    }
}
