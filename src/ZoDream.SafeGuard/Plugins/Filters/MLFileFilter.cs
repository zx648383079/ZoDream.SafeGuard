using System;
using System.IO;
using System.Threading;
using ZoDream.SafeGuard.DataNet;
using ZoDream.SafeGuard.Finders;

namespace ZoDream.SafeGuard.Plugins.Filters
{
    public class MLFileFilter(Train nn) : BaseFileFilter
    {
        private readonly Train _nn = nn;

        public override bool Valid(FileLoader fileInfo, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
