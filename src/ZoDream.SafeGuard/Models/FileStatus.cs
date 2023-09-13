using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.SafeGuard.Models
{
    public enum FileCheckStatus
    {
        None,
        Waiting,
        Checking,
        Normal,
        Poisoning, // 中毒
        Virus,    // 病毒
    }

    public enum FileTransformStatus
    {
        None,
        Waiting,
        Doing,
        Done,
    }
}
