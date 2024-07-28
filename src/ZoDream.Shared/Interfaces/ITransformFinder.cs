using System.Collections.Generic;
using System.Threading;
using ZoDream.Shared.Finders;

namespace ZoDream.Shared.Interfaces
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
