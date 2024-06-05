using ZoDream.Shared.Finders;
using ZoDream.Shared.Plugins.Filters;

namespace ZoDream.Shared.Plugins.Processes
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
                new TextRegexFileFilter(@"[\[\(]['""](\\x[\da-f]{2}|\\b\d){3,}['""][\]\)]")
                {
                    VaildStatus = Models.FileCheckStatus.Poisoning,
                }
            ];
        }

        public bool Process(ref FilterFinder finder)
        {
            finder.FilterItems = LoadFilters();
            return true;
        }
    }
}
