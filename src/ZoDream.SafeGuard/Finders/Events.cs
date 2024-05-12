using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.SafeGuard.Models;

namespace ZoDream.SafeGuard.Finders
{
    public delegate void FinderLogEventHandler(string fileName);

    public delegate void FinderFilterEventHandler(FileInfo item, FileCheckStatus status);
    public delegate void FinderEventHandler(FileInfo item);
    public delegate void FinderFinishedEventHandler();
}
