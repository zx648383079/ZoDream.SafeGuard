using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ZoDream.SafeGuard.Finders.Filters
{
    public class NameFileFilter : BaseFileFilter
    {

        public NameFileFilter(string pattern)
        {
            _nameRegex = new Regex(pattern, RegexOptions.IgnoreCase);
        }

        private readonly Regex _nameRegex;

        public override bool Valid(FileLoader fileInfo, CancellationToken token)
        {
            return _nameRegex.IsMatch(fileInfo.Name);
        }
    }
}
