using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.SafeGuard.Models;

namespace ZoDream.SafeGuard.Extensions
{
    public static class FileInfoItemExtension
    {
        public static bool Contains(this Collection<FileInfoItem> items, string fileName)
        {
            foreach (var item in items)
            {
                if (item.FileName == fileName)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool TryAdd(this Collection<FileInfoItem> items, string fileName)
        {
            if (!Contains(items, fileName))
            {
                items.Add(new FileInfoItem(fileName));
            }
            return true;
        }

        public static bool TryAdd(this Collection<FileInfoItem> items, IEnumerable<string> files)
        {
            foreach (var item in files)
            {
                items.TryAdd(item);
            }
            return true;
        }
    }
}
