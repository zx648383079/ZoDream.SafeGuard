﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Finders;

namespace ZoDream.Shared.Interfaces
{
    public interface IFilterFinder
    {
        public event FinderLogEventHandler? FileChanged;

        public event FinderFilterEventHandler? FoundChanged;

        public event FinderFinishedEventHandler? Finished;

        public void Start(IEnumerable<string> folders);

        public void Stop();
    }
}
