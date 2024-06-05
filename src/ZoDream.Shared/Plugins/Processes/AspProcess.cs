using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Plugins.Filters;

namespace ZoDream.Shared.Plugins.Processes
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
