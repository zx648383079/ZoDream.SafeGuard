using ZoDream.SafeGuard.Finders;

namespace ZoDream.SafeGuard.Plugins
{
    public interface IFileProcess
    {
        public string[] LoadExtension();

        public IFileFilter[] LoadFilters();

        public bool Process(ref FilterFinder finder);
    }
}
