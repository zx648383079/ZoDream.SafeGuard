﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.SafeGuard.Routes
{
    public interface IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, object> queries);
    }
}
