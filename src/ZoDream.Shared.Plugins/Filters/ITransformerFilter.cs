using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZoDream.Shared.Plugins.Filters
{
    public interface ITransformerFilter
    {
        public bool IsMatchFile(FileInfo fileInfo, CancellationToken token = default);
        public string TranformFileName(string fileName);

        public void Ready();
    }
}
