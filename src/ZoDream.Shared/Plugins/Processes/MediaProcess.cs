using ZoDream.Shared.Finders;
using ZoDream.Shared.Plugins.Filters;

namespace ZoDream.Shared.Plugins.Processes
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
                    ["<%", "execute", "%>"],
                    ["<?php", "eval",],
                    ["<?=", "eval", "?>"]
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
