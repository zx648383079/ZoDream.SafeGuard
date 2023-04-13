using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZoDream.SafeGuard.Finders.Filters
{
    public class SameFileFilter : BaseFileFilter
    {
        public SameFileFilter(IEnumerable<FileInfo> exampleItems)
        {
            foreach (var item in exampleItems)
            {
                _exampleItems.Add(new FileLoader(item));
            }
        }

        public SameFileFilter(IEnumerable<string> exampleItems)
        {
            foreach (var item in exampleItems)
            {
                var info = new FileInfo(item);
                if (info.Exists)
                {
                    _exampleItems.Add(new FileLoader(info));
                    continue;
                }
                FileLoader.EachFiles(new DirectoryInfo(item), data => {
                    _exampleItems.Add(new FileLoader(data));
                });
            }
        }

        protected readonly IList<FileLoader> _exampleItems = new List<FileLoader>();

        public override bool Valid(FileLoader fileInfo, CancellationToken token)
        {
            foreach (var item in _exampleItems)
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }
                if (ValidFile(item, fileInfo))
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual bool ValidFile(FileLoader example,  FileLoader file)
        {
            if (example.Length != file.Length)
            {
                return false;
            }
            return example.Md5 == file.Md5;
        } 
        
    }
}
