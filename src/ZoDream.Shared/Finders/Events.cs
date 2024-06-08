using ZoDream.Shared.Models;

namespace ZoDream.Shared.Finders
{
    public delegate void FinderLogEventHandler(string fileName);

    public delegate void FinderFilterEventHandler(FileInfo item, FileCheckStatus status);
    public delegate void FinderEventHandler(FileInfoItem item);
    public delegate void FinderFinishedEventHandler();
}
