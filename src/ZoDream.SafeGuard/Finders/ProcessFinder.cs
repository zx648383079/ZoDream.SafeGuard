﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ZoDream.SafeGuard.Models;
using ZoDream.SafeGuard.Plugins;

namespace ZoDream.SafeGuard.Finders
{
    public class ProcessFinder: BaseFilterFinder
    {
        public IList<IFileProcess>? ProcessItems { get; set; } = [];

        protected override FileCheckStatus CheckFileStatus(FileInfo fileInfo, CancellationToken token = default)
        {
            if (ProcessItems == null || ProcessItems.Count == 0)
            {
                return FileCheckStatus.Pass;
            }
            var extension = fileInfo.Extension[1..].ToLower();
            using var fileLoader = new FileLoader(fileInfo);
            var isMatch = false;
            foreach (var process in ProcessItems)
            {
                if (!process.LoadExtension().Contains(extension))
                {
                    continue;
                }
                isMatch = true;
                foreach (var item in process.LoadFilters())
                {
                    if (token.IsCancellationRequested)
                    {
                        return FileCheckStatus.Pass;
                    }
                    var status = item.Valid(fileLoader, token);
                    if (status > FileCheckStatus.Normal)
                    {
                        return status;
                    }
                }
            }
            return isMatch ? FileCheckStatus.Normal : FileCheckStatus.Pass;
        }
    }
}