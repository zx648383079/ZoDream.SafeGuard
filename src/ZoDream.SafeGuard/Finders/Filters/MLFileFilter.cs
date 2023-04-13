using System;
using System.IO;
using System.Threading;
using ZoDream.SafeGuard.DataNet;

namespace ZoDream.SafeGuard.Finders.Filters
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
