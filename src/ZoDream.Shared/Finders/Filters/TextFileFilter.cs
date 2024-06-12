using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Finders;

namespace ZoDream.Shared.Finders.Filters
{
    public class TextFileFilter : BaseFileFilter
    {
        public TextFileFilter(string text)
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
            var reader = fileInfo.Reader;
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            var i = 0;
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }
                if (i >= _lines.Count)
                {
                    return true;
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
                line = line.Replace("\n", string.Empty).Replace("\r", string.Empty);
                if (i < 1)
                {
                    if (line == _lines[i] || line.EndsWith(_lines[i]))
                    {
                        i++;
                        continue;
                    }
                    continue;
                }
                if (line == _lines[i])
                {
                    i++;
                    continue;
                }
                if (line == _lines[i] || line.EndsWith(_lines[i]))
                {
                    i = 1;
                    continue;
                }
                i = 0;
            }
            return i >= _lines.Count;
        }
    }
}
