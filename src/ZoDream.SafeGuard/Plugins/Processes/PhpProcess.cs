﻿using ZoDream.SafeGuard.Finders;
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
