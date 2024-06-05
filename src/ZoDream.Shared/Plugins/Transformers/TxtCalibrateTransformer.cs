using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Plugins.Transformers
{
    public partial class TxtCalibrateTransformer : IFileTransformer, IFilePreprocess
    {
        const string RuleFileName = "_rules";

        /// <summary>
        /// 是否需要自动合并章节
        /// </summary>
        public bool MergeEnabled { get; private set; } = false;
        public List<object> RuleItems { get; private set; } = [];

        public IEnumerable<string> Preprocess(IEnumerable<string> files)
        {
            MergeEnabled = false;
            RuleItems.Clear();
            return files.Where(item => {
                if (File.Exists(item) && 
                Path.GetFileName(item).StartsWith(RuleFileName))
                {
                    LoadRules(item);
                    return false;
                }
                return true;
            });
        }

        public bool IsMatch(FileInfo file)
        {
            return file.Extension == ".txt";
        }

        public string? Transform(string content)
        {
            //content = content.Replace("\r\n", "\n").Replace('「', '“')
            //    .Replace('」', '”');
            //content = content.Replace("”“", "”\n    “");
            //content = CommaRegex().Replace(content, "”\n    ");
            //content = CommaLineRegex().Replace(content, "”\n    “");
            var lineTag = content.Contains("\r\n") ? "\r\n" : "\n";
            var lines =  content.Split(lineTag);
            var sb = new StringBuilder();
            var lastIsContent = false;
            var lastNotEnd = false;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                if (!IsWhitespaceStart(line))
                {
                    if (!MergeEnabled)
                    {
                        sb.Append(lineTag);
                        if (lastIsContent)
                        {
                            sb.Append(lineTag);
                            sb.Append(lineTag);
                        }
                        sb.Append(line);
                        lastIsContent = false;
                        lastNotEnd = false;
                    }
                    continue;
                }
                var str = line.Trim();
                if (!str.Contains('“') && !str.Contains('”'))
                {
                    str = str.Replace('「', '“').Replace('」', '”');
                }
                if (str[0] == '”')
                {
                    str = str[1..];
                    if (lastIsContent)
                    {
                        sb.Append('”');
                    }
                    lastNotEnd = false;
                }
                str = RemoveAnything(str);
                if (IsJustSeparator(str))
                {
                    continue;
                }
                var blocks = str.Split("”“");
                for (int j = 0; j < blocks.Length; j++)
                {
                    var item = blocks[j];
                    if (j > 0)
                    {
                        item = "“" + item;
                    }
                    if (j < blocks.Length - 1)
                    {
                        item += "”";
                    }
                    if (lastNotEnd && item.StartsWith('“'))
                    {
                        lastNotEnd = false;
                        sb.Append('。');
                    }
                    if (lastNotEnd)
                    {
                        sb.Append(item);
                    }
                    else
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append(lineTag);
                        }
                        sb.Append($"    {item}");
                    }
                    lastIsContent = true;
                    lastNotEnd = !HasEndChar(item);
                }
            }
            return sb.ToString();
        }

        private string RemoveAnything(string line)
        {
            foreach (var item in RuleItems)
            {
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }
                if (item is Regex r)
                {
                    line = r.Replace(line, string.Empty);
                } else if (item is string s)
                {
                    line = line.Replace(s, string.Empty);
                }
            }
            return line;
        }

        private static bool IsJustSeparator(string line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (!IsSeparator(line[i]) && !IsWhitespace(line[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool HasEndChar(string line)
        {
            if (line.Length == 0)
            {
                return true;
            }
            if (!IsEndChat(line))
            {
                return IsEndSeparator(line[^1]);
            }
            return IsSeparator(line[^1]);
        }

        private static bool IsEndChat(string line)
        {
            var i = line.LastIndexOf('“');
            return i < 0 || line.IndexOf('”', i) > 0; 
        }

        private static bool IsSeparator(char code)
        {
            if (IsEndSeparator(code))
            {
                return true;
            }
            return code is '.' or ',' or '，' or '：' or
                ':' 
                or '）'
                or '…' or '＊' or '*' or '】' or '~';
        }

        private static bool IsEndSeparator(char code)
        {
            return code is '。'
                or '”' or '？' or '？' or '！'
                or '?' or '!';
        }

        private static bool IsWhitespaceStart(string line)
        {
            if (line.Length == 0)
            {
                return false;
            }
            return IsWhitespace(line[0]);
        }

        private static bool IsWhitespace(char code)
        {
            return code is '　' or '\t' or ' ' or '\r' or '\n';
        }

        private void LoadRules(string fileName)
        {
            var reader = LocationStorage.Reader(fileName);
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }
                line = line.Trim();
                if (line.Length == 0)
                {
                    continue;
                }
                if (line.IndexOf(": ") != 1)
                {
                    RuleItems.Add(line);
                    continue;
                }
                var tag = line[..1];
                line = line[3..].Trim();
                switch (tag)
                {
                    case "$":
                        RuleItems.Add(new Regex(line));
                        break;
                    case "@":
                        MergeEnabled = true;
                        break;
                    default:
                        break;
                }
            }
        }

        
    }
}
