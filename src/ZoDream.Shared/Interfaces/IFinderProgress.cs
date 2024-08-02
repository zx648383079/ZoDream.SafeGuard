using ZoDream.Shared.Finders;

namespace ZoDream.Shared.Interfaces
{
    public interface IFinderProgress
    {
        public event FinderProgressEventHandler? FileProgress;
    }
}
