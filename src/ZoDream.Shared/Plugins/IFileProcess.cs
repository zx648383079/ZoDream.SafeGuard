using ZoDream.Shared.Finders;

namespace ZoDream.Shared.Plugins
{
    public interface IFileProcess
    {
        public string[] LoadExtension();

        public IFileFilter[] LoadFilters();

        public bool Process(ref FilterFinder finder);
    }
}
