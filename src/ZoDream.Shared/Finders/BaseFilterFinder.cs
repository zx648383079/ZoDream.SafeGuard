using System.IO;
using System.Threading;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Finders
{
    public abstract class BaseFilterFinder: StorageFinder, IFilterFinder
    {
        public event FinderFilterEventHandler? FoundChanged;

        protected override void CheckFile(FileInfo file, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }
            OnCheckingFile(file);
            var status = CheckFileStatus(file, token);
            if (status <= FileCheckStatus.Normal)
            {
                return;
            }
            FoundChanged?.Invoke(file, status);
            ProcessFile(file, token);
        }

        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            return CheckFileStatus(fileInfo, token) > FileCheckStatus.Normal;
        }

        protected abstract FileCheckStatus CheckFileStatus(FileInfo fileInfo, CancellationToken token = default);
    }
}
