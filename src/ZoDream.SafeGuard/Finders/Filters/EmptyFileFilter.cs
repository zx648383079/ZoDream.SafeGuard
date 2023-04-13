using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZoDream.SafeGuard.Finders.Filters
{
    public class EmptyFileFilter : BaseFileFilter
    {
        public EmptyFileFilter(bool ignore = false)
        {
            _ignore = ignore;
        }

        private bool _ignore;

        public override bool Valid(FileLoader fileInfo, CancellationToken token)
        {
            var res = fileInfo.Length <= 0;
            return _ignore ? !res : res;
        }
    }
}
