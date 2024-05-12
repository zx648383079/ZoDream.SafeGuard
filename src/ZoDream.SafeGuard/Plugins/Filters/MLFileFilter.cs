using System;
using System.IO;
using System.Threading;
using ZoDream.SafeGuard.DataNet;
using ZoDream.SafeGuard.Finders;

namespace ZoDream.SafeGuard.Plugins.Filters
{
    public class MLFileFilter(Train nn) : BaseFileFilter
    {
        public override bool IsValid(FileLoader fileInfo, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
