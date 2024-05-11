using ZoDream.SafeGuard.Finders;
using ZoDream.SafeGuard.Plugins.Filters;

namespace ZoDream.SafeGuard.Plugins.Processes
{
    public class PhpProcess : IFileProcess
    {
        public string[] LoadExtension()
        {
            return ["php", "phtml"];
        }

        public IFileFilter[] LoadFilters()
        {
            return [
                new TextKeywordFileFilter("evel("),
                new Base64FileFilter(),
            ];
        }

        public bool Process(ref FilterFinder finder)
        {
            finder.FilterItems = LoadFilters();
            return true;
        }
    }
}
