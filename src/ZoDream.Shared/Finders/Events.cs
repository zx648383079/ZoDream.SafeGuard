using System.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Finders
{
    public delegate void FinderLogEventHandler(string fileName);
    public delegate void FinderProgressEventHandler(string fileName, long current, long total);

    public delegate void FinderFilterEventHandler(FileInfo item, FileCheckStatus status);
    public delegate void FinderEventHandler<T>(T item) where T : FileInfoItem;
    public delegate void FinderEventHandler(FileInfoItem item);
    public delegate void FinderFinishedEventHandler();
}
