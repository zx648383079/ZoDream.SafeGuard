using ZoDream.SafeGuard.Finders;
using ZoDream.SafeGuard.Plugins.Filters;

namespace ZoDream.SafeGuard.Plugins.Processes
{
    public class JavascriptProcess : IFileProcess
    {
        public string[] LoadExtension()
        {
            return ["js", "ts"];
        }

        public IFileFilter[] LoadFilters()
        {
            return [
                new TextRegexFileFilter(@"[\[\(]['""](\\x[\da-f]{2}|\\b\d)+['""][\]\)]")
            ];
        }

        public bool Process(ref FilterFinder finder)
        {
            finder.FilterItems = LoadFilters();
            return true;
        }
    }
}
