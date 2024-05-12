using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.SafeGuard.Plugins
{
    public class TextByteFinder: ByteFinder
    {
        public TextByteFinder(string text): this(text, Encoding.ASCII)
        {
            
        }

        public TextByteFinder(string text, Encoding encoding)
        {
            _items = encoding.GetBytes(text);
        }

        private readonly byte[] _items;

        protected override long VerifyLength => _items.LongLength;

        protected override byte GetByte(long index)
        {
            return _items[index];
        }

        protected override bool IsInvalidByte(byte code)
        {
            return !_items.Contains(code);
        }
    }
}
