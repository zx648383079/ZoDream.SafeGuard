using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Storage;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ZoDream.Shared.TextNetwork
{
    public partial class CharDictionary
    {
        private static readonly Lazy<CharDictionary> _lazy = new();
        public static CharDictionary Instance => _lazy.Value;

        private readonly Dictionary<char, uint> _charDict = [];
        private readonly Dictionary<uint, char> _codeDict = [];
        private uint _nextIndex;

        public void TryAdd(char code)
        {
            TryAdd(code, out var _);
        }

        public bool TryAdd(char code, out uint index)
        {
            index = _nextIndex;
            Add(code, index);
            return true;
        }

        private void Add(char code, uint index)
        {
            if (_charDict.ContainsKey(code))
            {
                return;
            }
            if (_codeDict.ContainsKey(index))
            {
                index = _nextIndex;
            }
            _charDict.Add(code, index);
            _codeDict.Add(index, code);
            _nextIndex = Math.Max(index, _nextIndex) + 1;
        }

        public char? ToChar(uint code)
        {
            if (_codeDict.TryGetValue(code, out var result))
            {
                return result; 
            }
            return null;
        }

        public uint ToInt(char c) 
        {
            if (_charDict.TryGetValue(c, out var result))
            {
                return result;
            }
            if (TryAdd(c, out var index))
            {
                return index;
            }
            return 0;
        }

        public uint[] ToInt(string val)
        {
            var items = new List<uint>();
            for (var i = 0; i < val.Length; i++)
            {
                items.Add(ToInt(val[i]));
            }
            return [..items];
        }

        public string ToChar(IEnumerable<uint> items)
        {
            var sb = new StringBuilder();
            foreach (var item in items)
            {
                sb.Append(ToChar(item));
            }
            return sb.ToString();
        }

        public async Task LoadAsync(string fileName)
        {
            using var reader = LocationStorage.Reader(fileName);
            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                var args = line.Split([' '], 2);
                Add(args[1][0], Convert.ToUInt32(args[0]));
            }
        }

        public Task LoadFromTxtAsync(string fileName)
        {
            return Task.Factory.StartNew(() => {
                var data = new Dictionary<char, int>();
                LoadFromTxt(fileName, ref data);
                Add(data);
            });
        }

        public Task LoadFromFolderAsync(string fileName)
        {
            return Task.Factory.StartNew(() => {
                var data = new Dictionary<char, int>();
                StorageFinder.EachFiles(fileName, items => {
                    foreach (var item in items)
                    {
                        if (!item.EndsWith(".txt"))
                        {
                            continue;
                        }
                        LoadFromTxt(item, ref data);
                    }
                });
                Add(data);
            });
        }

        private void Add(Dictionary<char, int> data)
        {
            var items = data.OrderByDescending(item => item.Value).Select(item => item.Key);
            foreach (var item in items)
            {
                TryAdd(item);
            }
        }

        private void LoadFromTxt(string fileName, ref Dictionary<char, int> data)
        {
            using var reader = LocationStorage.Reader(fileName);
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }
                // line = ChineseConverter.Convert(line, ChineseConversionDirection.TraditionalToSimplified);
                char? last = null;
                foreach (var item in line)
                {
                    var code = FormatNotation(item);
                    if (last == code && IsNotation(code))
                    {
                        continue;
                    }
                    if (data.TryGetValue(code, out int value))
                    {
                        data[code] = ++value;
                    }
                    else
                    {
                        data.Add(code, 1);
                    }
                }
            }
        }

        private bool IsNotation(char code)
        {
            return code switch
            {
                ',' or '.' or '!' or '?' or ';' or '_' or ':' or ' ' or '/' or '*' or '…' => true,
                _ => false
            };
        }

        private char FormatNotation(char code)
        {
            if (code >= 65281 && code <= 65374)
            {
                // 全角字符转换
                return Convert.ToChar(code - 65281 + 33);
            }
            if (code >= 9312 && code <= 9320)
            {
                // 1 - 20 的变体 9312 - 9371 
                // a-z 的变体 9372- 9449
                return Convert.ToChar(code - 9312 + 49);
            }
            return code switch
            {
                '，' => ',',
                '。' => '.',
                '！' => '!',
                '？' => '?',
                '﹖' => '?',
                '；' => ';',
                '︰' => ':',
                '：' => ':',
                '（' => '(',
                '）' => ')',
                '〔' => '(',
                '〕' => ')',
                '【' => '[',
                '】' => ']',
                '『' => '[',
                '「' => '[',
                '」' => ']',
                '』' => ']',
                '《' => '<',
                '》' => '>',
                '　' => ' ',
                '﹔' => ';',
                '﹐' => ',',
                '●' => '•',
                '·' => '•',
                '※' => '*',
                '〃' => '"',
                '\t' => ' ',
                '\r' => ' ',
                '\n' => ' ',
                _ => code,
            };
        }


        public async Task SaveAsync(string fileName) 
        {
            using var writer = LocationStorage.Writer(fileName);
            foreach (var item in _charDict) 
            {
                var val = item.Key.ToString();
                var target = ChineseConverter.Convert(val, ChineseConversionDirection.TraditionalToSimplified);
                if (target == val)
                {
                    await writer.WriteLineAsync($"{item.Value} {item.Key}");
                }
                else
                {
                    await writer.WriteLineAsync($"{item.Value} {item.Key} {target}");
                }

            }
        }

    }
}
