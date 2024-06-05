using System.Collections.Generic;
using System.Threading;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Plugins
{
    public interface IFileFilter
    {
        public FileCheckStatus Valid(FileLoader fileInfo, CancellationToken token);

        public IList<FileLoader> Filter(IList<FileLoader> files, CancellationToken token);
    }
}
