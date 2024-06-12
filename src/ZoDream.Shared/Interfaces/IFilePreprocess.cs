using System.Collections.Generic;

namespace ZoDream.Shared.Interfaces
{
    public interface IFilePreprocess
    {

        public IEnumerable<string> Preprocess(IEnumerable<string> files);
    }
}
