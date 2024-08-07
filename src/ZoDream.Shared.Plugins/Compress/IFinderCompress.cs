namespace ZoDream.Shared.Plugins.Compress
{
    public interface IFinderCompress
    {
        public string OutputFolder { set; }
        public string DictionaryFileName { set; }
        public string Password { set; }

        public bool Multiple { set; }
    }
}
