using System.Text.RegularExpressions;
using System.Threading;
using ZoDream.SafeGuard.Finders;

namespace ZoDream.SafeGuard.Plugins.Filters
{
    public class NameFileFilter(string pattern) : BaseFileFilter
    {
        private readonly Regex _nameRegex = new(pattern, RegexOptions.IgnoreCase);

        public override bool Valid(FileLoader fileInfo, CancellationToken token)
        {
            return _nameRegex.IsMatch(fileInfo.Name);
        }
    }
}
