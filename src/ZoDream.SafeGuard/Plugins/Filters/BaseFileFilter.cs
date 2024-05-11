using System.Collections.Generic;
using System.Threading;
using ZoDream.SafeGuard.Finders;

namespace ZoDream.SafeGuard.Plugins.Filters
{
    public abstract class BaseFileFilter : IFileFilter
    {
        public IList<FileLoader> Filter(IList<FileLoader> files, CancellationToken token)
        {
            var items = new List<FileLoader>();
            foreach (var file in files)
            {
                if (token.IsCancellationRequested)
                {
                    return items;
                }
                if (Valid(file, token))
                {
                    items.Add(file);
                }
            }
            return items;
        }

        public abstract bool Valid(FileLoader fileInfo, CancellationToken token);
    }
}
