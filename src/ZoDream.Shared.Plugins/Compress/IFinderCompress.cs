using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Plugins.Compress
{
    public interface IFinderCompress: IFinderOutput
    {
        public string DictionaryFileName { set; }
        public string Password { set; }

        public bool Multiple { set; }
    }
}
