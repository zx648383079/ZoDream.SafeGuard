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
    public class HiddenFileFilter : BaseFileFilter
    {
        public HiddenFileFilter(bool ignore = false)
        {
            _ignore = ignore;
        }

        private readonly bool _ignore;

        public override bool Valid(FileLoader fileInfo, CancellationToken token)
        {
            var res = fileInfo.File.Attributes == FileAttributes.Hidden;
            return _ignore ? !res : res;
        }
    }
}
