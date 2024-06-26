﻿using System.Collections.Generic;
using System.IO;
using System.Threading;
using ZoDream.Shared.Finders;

namespace ZoDream.Shared.Finders.Filters
{
    /// <summary>
    /// 关键字搜索
    /// </summary>
    public class TextKeywordFileFilter : BaseFileFilter
    {
        public TextKeywordFileFilter(string text)
        {
            foreach (var item in text.Split(['\n', '\r']))
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

        private readonly IList<string> _lines = [];
        public override bool IsValid(FileLoader fileInfo, CancellationToken token)
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
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
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
