using ZoDream.Shared.Finders;
using ZoDream.Shared.Finders.Filters;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.CodeScanner.Processes
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
                new TextKeywordFileFilter("eval(")
                {
                    VaildStatus = Models.FileCheckStatus.Poisoning,
                },
                new Base64FileFilter()
                {
                    VaildStatus = Models.FileCheckStatus.Poisoning,
                },
            ];
        }

        public bool Process(ref FilterFinder finder)
        {
            finder.FilterItems = LoadFilters();
            return true;
        }
    }
}
