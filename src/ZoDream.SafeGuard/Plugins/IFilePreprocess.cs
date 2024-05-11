using System.Collections.Generic;

namespace ZoDream.SafeGuard.Plugins
{
    public interface IFilePreprocess
    {

        public IEnumerable<string> Preprocess(IEnumerable<string> files);
    }
}
