using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using ZoDream.Shared.Finders;

namespace ZoDream.Shared.Plugins.Filters
{
    /// <summary>
    /// 正则匹配
    /// </summary>
    public class TextRegexFileFilter : BaseFileFilter
    {
        public TextRegexFileFilter(string text)
        {
            foreach (var item in text.Split(['\n', '\r']))
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

        private readonly IList<Regex> _lines = [];
        public override bool IsValid(FileLoader fileInfo, CancellationToken token)
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
