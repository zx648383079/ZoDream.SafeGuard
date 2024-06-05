using ZoDream.Shared.Plugins;

namespace ZoDream.Shared.Finders
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
