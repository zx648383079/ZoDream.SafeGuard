using System;
using System.IO;
using System.Threading;
using ZoDream.SafeGuard.DataNet;
using ZoDream.SafeGuard.Finders;

namespace ZoDream.SafeGuard.Plugins.Filters
{
    public class MLFileFilter : BaseFileFilter
    {
        public MLFileFilter(Train nn)
        {
            _nn = nn;
        }

        private readonly Train _nn;

        public override bool Valid(FileLoader fileInfo, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
