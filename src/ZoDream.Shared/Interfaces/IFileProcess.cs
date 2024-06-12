using ZoDream.Shared.Finders;

namespace ZoDream.Shared.Interfaces
{
    public interface IFileProcess
    {
        public string[] LoadExtension();

        public IFileFilter[] LoadFilters();

        public bool Process(ref FilterFinder finder);
    }
}
