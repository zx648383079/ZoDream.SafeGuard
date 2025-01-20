using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Storage;
using ZoDream.Shared.TextCalibrate.Formatters;

namespace ZoDream.Shared.TextCalibrate.Transformers
{
    public partial class TxtCalibrateTransformer : IFileTransformer, IFilePreprocess
    {
        const string RuleFileName = "_rules";

        private readonly char[] _chatCodes = ['“', '”'];
        private readonly char[] _endCodes = ['“', '”',  '!', '?', '.', '！', '？', '。'];

        /// <summary>
        /// 是否需要自动合并章节
        /// </summary>
        public bool MergeEnabled { get; private set; } = false;

        public bool? SimplifiedToTraditional = null;
        /// <summary>
        /// 只有超过多少字的才自动拆分行
        /// </summary>
        public int MinSplitLength = 0;

        public List<ITextFormatter> RuleItems { get; private set; } = [];

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
            if (SimplifiedToTraditional is not null)
            {
                content = ChineseConverter.Convert(content,
                    SimplifiedToTraditional == true ?
                    ChineseConversionDirection.SimplifiedToTraditional : ChineseConversionDirection.TraditionalToSimplified);
            }
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
                        if (sb.Length > 0)
                        {
                            sb.Append(lineTag);
                        }
                        if (lastIsContent)
                        {
                            sb.Append(lineTag);
                            sb.Append(lineTag);
                        }
                        sb.Append(ReplaceTitle(line));
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
                if (line.Length < MinSplitLength) 
                {
                    if (sb.Length != 0)
                    {
                        sb.Append(lineTag);
                    }
                    sb.Append($"    {str}");
                    lastIsContent = true;
                    lastNotEnd = false;
                    continue;
                }
                var start = 0;
                var isInlineIndex = 0;
                var blockIndex = 0;
                while (start < str.Length)
                {
                    blockIndex ++;
                    var beginIsChatStart = str[start] == _chatCodes[0];
                    var end = str.IndexOfAny(beginIsChatStart ? _chatCodes : _endCodes, beginIsChatStart ? start + 1 : start);
                    var endIsChatStart = end >= 0 && str[end] == _chatCodes[0];
                    if (end == 0 && endIsChatStart)
                    {
                        if (lastNotEnd)
                        {
                            lastNotEnd = false;
                            sb.Append('。');
                        }
                    }
                    if (end < 0)
                    {
                        end = str.Length;
                    } else if (!endIsChatStart)
                    {
                        end++;
                    }
                    var text = str[start..end];
                    if (lastNotEnd || (start > 0 && IsSaySeparator(str[start - 1])) || (isInlineIndex > 0 && blockIndex - isInlineIndex < 3))
                    {
                        sb.Append(text);
                    }
                    else
                    {
                        if (sb.Length != 0)
                        {
                            sb.Append(lineTag);
                        }
                        sb.Append($"    {text}");
                    }
                    lastIsContent = true;
                    lastNotEnd = !HasEndChar(text);
                    if (endIsChatStart)
                    {
                        isInlineIndex = lastNotEnd ? blockIndex : 0;
                    }
                    start = end;
                }
            }
            return sb.ToString();
        }


        private string ReplaceTitle(string line)
        {
            return TitleWhiteSpaceRegex().Replace(line.Trim(), " ");
        }

        private string RemoveAnything(string line)
        {
            foreach (var item in RuleItems)
            {
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }
                line = item.Format(line);
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
            if (IsSaySeparator(code))
            {
                return true;
            } 
            return code is '.' or ',' or '，' or '）'
                or '…' or '＊' or '*' or '】' or '~';
        }

        private static bool IsSaySeparator(char code)
        {
            return code is '：' or ':';
        }

        private static bool IsEndSeparator(char code)
        {
            return code is '。'
                or '”' or '？' or '！'
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
                    RuleItems.Add(new ReplaceFormatter(line));
                    continue;
                }
                var tag = line[..1];
                line = line[3..].Trim();
                switch (tag)
                {
                    case "$":
                    case "|":
                        {
                            var args = line.Split("<=>", 2);
                            var search = args[0];
                            if (string.IsNullOrEmpty(search))
                            {
                                break;
                            }
                            var replacement = args.Length == 2 ? args[1] : string.Empty;
                            try
                            {
                                RuleItems.Add(tag == "$" ?
                                new RegexReplaceFormatter(new Regex(search), replacement)
                                : new ReplaceFormatter(search, replacement));
                            }
                            catch (Exception)
                            {
                                break;
                            }
                        }
                        break;
                    case "@":
                        MergeEnabled = true;
                        break;
                    case "~":
                        SimplifiedToTraditional = line.ToLower() switch
                        {
                            "c" => false,
                            "t" => true,
                            _ => null
                        };
                        break;
                    case ">":
                        _ = int.TryParse(line, out MinSplitLength);
                        break;
                    default:
                        break;
                }
            }
        }

        [GeneratedRegex(@"\s{2,}")]
        private static partial Regex TitleWhiteSpaceRegex();
    }
}
