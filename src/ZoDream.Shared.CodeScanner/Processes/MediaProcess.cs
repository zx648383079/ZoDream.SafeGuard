using ZoDream.Shared.Finders;
using ZoDream.Shared.Finders.Filters;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.CodeScanner.Processes
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
