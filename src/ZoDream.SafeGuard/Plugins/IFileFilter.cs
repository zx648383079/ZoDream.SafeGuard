using System.Collections.Generic;
using System.Threading;
using ZoDream.SafeGuard.Finders;
using ZoDream.SafeGuard.Models;

namespace ZoDream.SafeGuard.Plugins
{
    public interface IFileFilter
    {
        public FileCheckStatus Valid(FileLoader fileInfo, CancellationToken token);

        public IList<FileLoader> Filter(IList<FileLoader> files, CancellationToken token);
    }
}
