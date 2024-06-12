using ZoDream.Shared.Finders;
using ZoDream.Shared.Finders.Filters;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.CodeScanner.Processes
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
