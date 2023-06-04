using System.Collections.Generic;
using System.Linq;

namespace ZoDream.SafeGuard.Finders.Filters
{
    public abstract class CharFileFilter
    {
        protected List<CharVerifyData> VerifyItems { get; set; } = new();

        protected abstract long VerifyLength { get; }

        protected CharVerifyData[] ValidItems => VerifyItems.Where(i => i.IsMatch).ToArray();
        protected int ValidCount => ValidItems.Length;

        public void Verify(int line, string lineText)
        {
            if (IsInvalidByte(lineText))
            {
                return;
            }

            int pos;

            int len;
            for (int i = VerifyItems.Count - 1; i >= 0; i--)
            {
                var item = VerifyItems[i];
                if (item.IsMatch)
                {
                    continue;
                }
                (pos, len) = Verify(lineText, (int)item.Length);
                if (pos < 0)
                {
                    VerifyItems.RemoveAt(i);
                    continue;
                }
                item.Length += len;
                if (item.Length == VerifyLength)
                {
                    item.EndLine = line;
                    item.EndColumn = pos + len;
                    item.IsMatch = true;
                }
            }
            (pos, len) = Verify(lineText, 0);
            if (pos < 0)
            {
                return;
            }
            VerifyItems.Add(new CharVerifyData()
            {
                BeginLine = line,
                BeginColumn = pos,
                Length = 1
            });
        }

        protected abstract string GetText(int line, int length);

        protected virtual bool IsInvalidByte(string line)
        {
            return string.IsNullOrWhiteSpace(line);
        }

        protected virtual (int, int) Verify(string line, int index)
        {
            if (index > 0)
            {
                var text = GetText(index, line.Length);
                return line.StartsWith(text) ? (0, text.Length) : (-1, 0);
            }
            var first = GetText(index, 1);
            var i = -1;
            while (true)
            {
                var pos = line.IndexOf(first, i);
                if (pos < 0)
                {
                    break;
                }
                if (GetText(1, line.Length - pos - 1) != line[pos..])
                {
                    break;
                }
                i = pos;
            }
            return i >= 0 ? (0, line.Length - i) : (-1, 0);
        }
    }

    public class CharVerifyData
    {
        public int BeginLine { get; set; }
        public int BeginColumn { get; set; }

        public int EndLine { get; set; }
        public int EndColumn { get; set; }

        public long Length { get; set; }

        public bool IsMatch { get; set; }
    }
}
