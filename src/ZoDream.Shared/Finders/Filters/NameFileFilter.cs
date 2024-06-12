using System.Text.RegularExpressions;
using System.Threading;
using ZoDream.Shared.Finders;

namespace ZoDream.Shared.Finders.Filters
{
    public class NameFileFilter(string pattern) : BaseFileFilter
    {
        private readonly Regex _nameRegex = new(pattern, RegexOptions.IgnoreCase);

        public override bool IsValid(FileLoader fileInfo, CancellationToken token)
        {
            return _nameRegex.IsMatch(fileInfo.Name);
        }
    }
}
