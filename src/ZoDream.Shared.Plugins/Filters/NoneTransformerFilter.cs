using System.IO;
using System.Threading;

namespace ZoDream.Shared.Plugins.Filters
{
    public class NoneTransformerFilter : ITransformerFilter
    {
        public void Ready()
        {

        }
        public bool IsMatchFile(FileInfo fileInfo, CancellationToken token = default)
        {
            return true;
        }

        public string TranformFileName(string fileName)
        {
            return fileName;
        }
    }
}
