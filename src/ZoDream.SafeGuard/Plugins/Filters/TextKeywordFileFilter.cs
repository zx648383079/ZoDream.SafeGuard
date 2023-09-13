using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.SafeGuard.Finders;

namespace ZoDream.SafeGuard.Plugins.Filters
{
    /// <summary>
    /// 关键字搜索
    /// </summary>
    public class TextKeywordFileFilter : BaseFileFilter
    {
        public TextKeywordFileFilter(string text)
        {
            foreach (var item in text.Split(new char[] { '\n', '\r' }))
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                var line = item.Trim();
                _length += line.Length;
                _lines.Add(line);
            }
        }

        private readonly int _length = 0;

        private readonly IList<string> _lines = new List<string>();
        public override bool Valid(FileLoader fileInfo, CancellationToken token)
        {
            if (_lines.Count == 0)
            {
                return false;
            }
            if (fileInfo.Length <= _length)
            {
                return false;
            }
            var matchRes = new int[_lines.Count];
            var reader = fileInfo.Reader;
            reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                for (int i = 0; i < _lines.Count; i++)
                {
                    if (line.Contains(_lines[i]))
                    {
                        matchRes[i]++;
                    }
                }
            }
            foreach (var item in matchRes)
            {
                if (item < 1)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
