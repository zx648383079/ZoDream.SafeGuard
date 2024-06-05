using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Models
{
    public enum FileCheckStatus
    {
        None,
        Waiting,
        Checking,
        Pass,
        /// <summary>
        /// 不检测到
        /// </summary>
        Normal,
        /// <summary>
        /// 检测到
        /// </summary>
        Valid,
        /// <summary>
        /// 中毒文件
        /// </summary>
        Poisoning,
        /// <summary>
        /// 病毒
        /// </summary>
        Virus,
    }

    public enum FileTransformStatus
    {
        None,
        Waiting,
        Doing,
        Done,
    }
}
