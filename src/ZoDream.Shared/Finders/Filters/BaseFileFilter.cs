using System.Collections.Generic;
using System.Threading;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Finders.Filters
{
    public abstract class BaseFileFilter : IFileFilter
    {

        public FileCheckStatus VaildStatus { private get; set; } = FileCheckStatus.Valid;

        public IList<FileLoader> Filter(IList<FileLoader> files, CancellationToken token)
        {
            var items = new List<FileLoader>();
            foreach (var file in files)
            {
                if (token.IsCancellationRequested)
                {
                    return items;
                }
                if (IsValid(file, token))
                {
                    items.Add(file);
                }
            }
            return items;
        }

        public FileCheckStatus Valid(FileLoader fileInfo, CancellationToken token)
        {
            return IsValid(fileInfo, token) ? VaildStatus : FileCheckStatus.Normal;
        }

        public abstract bool IsValid(FileLoader fileInfo, CancellationToken token);
    }
}
