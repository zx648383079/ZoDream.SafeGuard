﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Finders;

namespace ZoDream.Shared.Finders.Filters
{
    public class HiddenFileFilter(bool ignore = false) : BaseFileFilter
    {
        public override bool IsValid(FileLoader fileInfo, CancellationToken token)
        {
            var res = fileInfo.File.Attributes == FileAttributes.Hidden;
            return ignore ? !res : res;
        }
    }
}
