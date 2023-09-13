using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.SafeGuard.Finders;
using ZoDream.SafeGuard.Models;

namespace ZoDream.SafeGuard.Plugins
{
    public interface IFileFilter
    {
        public bool Valid(FileLoader fileInfo, CancellationToken token);

        public IList<FileLoader> Filter(IList<FileLoader> files, CancellationToken token);
    }
}
