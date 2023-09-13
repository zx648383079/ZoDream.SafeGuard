using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.SafeGuard.Finders;
using ZoDream.SafeGuard.Plugins;

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
