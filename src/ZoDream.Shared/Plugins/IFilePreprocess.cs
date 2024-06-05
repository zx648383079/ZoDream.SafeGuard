using System.Collections.Generic;

namespace ZoDream.Shared.Plugins
{
    public interface IFilePreprocess
    {

        public IEnumerable<string> Preprocess(IEnumerable<string> files);
    }
}
