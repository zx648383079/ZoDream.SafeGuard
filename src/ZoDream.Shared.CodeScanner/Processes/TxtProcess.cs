using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Finders.Filters;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.CodeScanner.Processes
{
    public class TxtProcess : IFileProcess
    {
        public string[] LoadExtension()
        {
            return ["txt"];
        }

        public IFileFilter[] LoadFilters()
        {
            return [
                new NameFileFilter(@"readme\.txt") {
                    VaildStatus = Models.FileCheckStatus.Pass,
                },
                new TextKeywordFileFilter("Hacked\nAttacker") {
                    VaildStatus = Models.FileCheckStatus.Poisoning
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
