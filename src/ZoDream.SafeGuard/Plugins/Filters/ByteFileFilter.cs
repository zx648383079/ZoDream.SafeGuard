using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.SafeGuard.Plugins.Filters
{
    public abstract class ByteFileFilter
    {

        protected List<ByteVerifyData> VerifyItems { get; set; } = new();

        protected abstract long VerifyLength { get; }

        protected ByteVerifyData[] ValidItems => VerifyItems.Where(i => i.IsMatch).ToArray();
        protected int ValidCount => ValidItems.Length;

        public void Verify(long position, byte code)
        {
            if (IsInvalidByte(code))
            {
                return;
            }
            for (int i = VerifyItems.Count - 1; i >= 0; i--)
            {
                var item = VerifyItems[i];
                if (item.IsMatch)
                {
                    continue;
                }
                if (!Verify(code, item.Length))
                {
                    VerifyItems.RemoveAt(i);
                    continue;
                }
                item.Length++;
                if (item.Length == VerifyLength)
                {
                    item.End = position;
                    item.IsMatch = true;
                }
            }
            if (!Verify(code, 0L))
            {
                return;
            }
            VerifyItems.Add(new ByteVerifyData()
            {
                Begin = position,
                Length = 1
            });
        }



        protected abstract byte GetByte(long index);
        protected abstract bool IsInvalidByte(byte code);

        protected virtual bool Verify(byte code, long index)
        {
            return GetByte(index) == code;
        }
    }

    public class ByteVerifyData
    {
        public long Begin { get; set; }

        public long End { get; set; }

        public long Length { get; set; }

        public bool IsMatch { get; set; }
    }
}
