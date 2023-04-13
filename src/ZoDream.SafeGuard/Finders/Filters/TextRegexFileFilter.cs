using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ZoDream.SafeGuard.Finders.Filters
{
    /// <summary>
    /// 正则匹配
    /// </summary>
    public class TextRegexFileFilter : BaseFileFilter
    {
        public TextRegexFileFilter(string text)
        {
            foreach (var item in text.Split(new char[] { '\n', '\r'}))
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                try
                {
                    _lines.Add(new Regex(item.Trim()));
                }
                catch (Exception)
                {
                }
            }
        }

        private readonly IList<Regex> _lines = new List<Regex>();
        public override bool Valid(FileLoader fileInfo, CancellationToken token)
        {
            if (_lines.Count == 0)
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
                    if (_lines[i].IsMatch(line))
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
