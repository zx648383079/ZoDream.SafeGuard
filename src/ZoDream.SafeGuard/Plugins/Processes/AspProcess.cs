using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.SafeGuard.Finders;
using ZoDream.SafeGuard.Plugins.Filters;

namespace ZoDream.SafeGuard.Plugins.Processes
{
    public class AspProcess : IFileProcess
    {
        public string[] LoadExtension()
        {
            return ["asp", "aspx"];
        }

        public IFileFilter[] LoadFilters()
        {
            return [
                new TextKeywordFileFilter("Request.BinaryRead(") {
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
