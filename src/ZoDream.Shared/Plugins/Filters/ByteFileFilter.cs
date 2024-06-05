using System.Threading;
using ZoDream.Shared.Finders;

namespace ZoDream.Shared.Plugins.Filters
{
    public class ByteFileFilter(StreamFinder finder) : BaseFileFilter
    {
        public ByteFileFilter(string[][] items): this(new StreamFinder(items))
        {
            
        }

        private readonly StreamFinder _finder = finder;

        public override bool IsValid(FileLoader fileInfo, CancellationToken token)
        {
            _finder.IsMatchFirst = true;
            var reader = fileInfo.Reader;
            reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            return _finder.MatchFile(reader.BaseStream);
        }
    }
}
