using System;
using System.Collections;
using System.Collections.Generic;

namespace ZoDream.Shared.TextCalibrate
{
    public class CharIterator(string content) : IEnumerator<char>, IEnumerable<char>
    {

        public string Content => content;
        public int Position { get; set; } = -1;

        public int Length => content.Length;

        public char Current => content[Position];

        object IEnumerator.Current => content[Position];

        public void Dispose()
        {

        }

        public IEnumerator<char> GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            if (!CanNext)
            {
                return false;
            }
            Position++;
            return true;
        }

        public void Reset()
        {
            Position = -1;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        public bool CanNext => Position < content.Length - 1;
        public bool CanBack => Position > 1;

        public int IndexOf(char c, int offset = 0)
        {
            return content.IndexOf(c, Position + offset);
        }
        public int IndexOf(string s, int offset = 0)
        {
            return content.IndexOf(s, Position + offset);
        }

        public string Read(int length = 1, int offset = 0)
        {
            if (length == 0)
            {
                return string.Empty;
            }
            var pos = (length < 0 ? Position + length : Position) + offset;
            if (pos > content.Length - 1)
            {
                return string.Empty;
            }
            var len = length < 0 ? -length : length;
            return content.Substring(pos, len);
        }

        public string ReadSeek(int position, int length = 1)
        {
            return content.Substring(position, length);
        }

        public bool NextIs(params char[] items)
        {
            if (!CanNext)
            {
                return false;
            }
            var c = content[Position + 1];
            foreach (var item in items)
            {
                if (c == item)
                {
                    return true;
                }
            }
            return false;
        }

        public bool NextIs(params string[] items)
        {
            if (!CanNext)
            {
                return false;
            }
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                if (content.Substring(Position + 1, item.Length) == item)
                {
                    return true;
                }
            }
            return false;
        }

        public int MinIndex(params char[] items)
        {
            var index = -1;
            var min = -1;
            for (int i = items.Length - 1; i >= 0; i--)
            {
                var j = IndexOf(items[i]);
                if (j >= 0 && (min < 0 || j <= min))
                {
                    index = i;
                    min = j;
                }
            }
            return index;
        }

        public int MinIndex(params string[] items)
        {
            var index = -1;
            var min = -1;
            for (int i = items.Length - 1; i >= 0; i--)
            {
                var j = IndexOf(items[i]);
                if (j >= 0 && (min < 0 || j <= min))
                {
                    index = i;
                    min = j;
                }
            }
            return index;
        }

        /// <summary>
        /// 反向遍历，不移动当前位置
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="offset">默认从前一个位置开始</param>
        public void Reverse(Func<char, int, bool?> cb, int offset = -1)
        {
            var i = Position + offset;
            while (i >= 0)
            {
                if (cb(content[i], i) == false)
                {
                    break;
                }
                i--;
            }
        }

        /// <summary>
        /// 字符是否是上一个字符，并计算连续出现的次数
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public int ReverseCount(char code)
        {
            var count = 0;
            Reverse((i, _) => {
                if (i != code)
                {
                    return false;
                }
                count++;
                return null;
            });
            return count;
        }
    }
}
