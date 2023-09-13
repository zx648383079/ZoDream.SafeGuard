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
    public class SizeFileFilter : BaseFileFilter
    {
        public SizeFileFilter(long min, long max = 0)
        {
            _minLength = min;
            _maxLength = max;
        }

        private readonly long _minLength;

        private readonly long _maxLength;

        public override bool Valid(FileLoader fileInfo, CancellationToken token)
        {
            var len = fileInfo.Length;
            if (len < _minLength)
            {
                return false;
            }
            return _maxLength > 0 && len <= _maxLength;
        }
    }
}
