using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.SafeGuard.Finders;

namespace ZoDream.SafeGuard.Plugins.Filters
{
    public class SizeFileFilter(long min, long max = 0) : BaseFileFilter
    {
        public override bool IsValid(FileLoader fileInfo, CancellationToken token)
        {
            var len = fileInfo.Length;
            if (len < min)
            {
                return false;
            }
            return max > 0 && len <= max;
        }
    }
}
