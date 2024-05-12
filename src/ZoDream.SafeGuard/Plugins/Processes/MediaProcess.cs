using ZoDream.SafeGuard.Finders;
using ZoDream.SafeGuard.Plugins.Filters;

namespace ZoDream.SafeGuard.Plugins.Processes
{
    public class MediaProcess : IFileProcess
    {
        public string[] LoadExtension()
        {
            return ["pn", "jpeg", "webp", "bmp", "gif", "jpg"];
        }

        public IFileFilter[] LoadFilters()
        {
            return [
                new ByteFileFilter([
                    ["<%", "%>"],
                    ["<?php"],
                    ["<?=", "?>"]
                ])
                {
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
