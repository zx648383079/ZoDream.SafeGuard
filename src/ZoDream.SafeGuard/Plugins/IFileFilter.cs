using System.Collections.Generic;
using System.Threading;
using ZoDream.SafeGuard.Finders;

namespace ZoDream.SafeGuard.Plugins
{
    public interface IFileFilter
    {
        public bool Valid(FileLoader fileInfo, CancellationToken token);

        public IList<FileLoader> Filter(IList<FileLoader> files, CancellationToken token);
    }
}
