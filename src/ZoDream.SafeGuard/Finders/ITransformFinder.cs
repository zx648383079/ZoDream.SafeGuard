using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.SafeGuard.Plugins;

namespace ZoDream.SafeGuard.Finders
{
    public interface ITransformFinder
    {
        public event FinderLogEventHandler? FileChanged;

        public event FinderEventHandler? FoundChanged;

        public event FinderFinishedEventHandler? Finished;

        public void Start(IEnumerable<string> folders);

        public void Stop();

        public string Transform(string content, CancellationToken token = default);

        public void Clear();

        public void Add(IFileTransformer transformer);
    }
}
