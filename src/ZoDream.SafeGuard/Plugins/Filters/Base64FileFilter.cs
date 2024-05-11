﻿using System.Text.RegularExpressions;
using System.Threading;
using ZoDream.SafeGuard.Finders;

namespace ZoDream.SafeGuard.Plugins.Filters
{
    /// <summary>
    /// Base64
    /// </summary>
    public partial class Base64FileFilter : BaseFileFilter
    {
        public override bool Valid(FileLoader fileInfo, CancellationToken token)
        {
            var reader = fileInfo.Reader;
            reader.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            var wordLength = 0L;
            var unsafeLength = 0L;
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
                line = line.Trim();
                wordLength += line.Replace(" ", string.Empty).Length;
                if (!line.Contains('\'') && !line.Contains('"')) 
                {
                    if (IsBase64String(line))
                    {
                        unsafeLength += line.Length;
                    }
                    continue;
                }
                foreach (var item in line.Split(['\'' , '"']))
                {
                    if (IsBase64String(item))
                    {
                        unsafeLength += item.Length;
                    }
                }
            }
            return unsafeLength > wordLength / 2;
        }

        private static bool IsBase64String(string val)
        {
            return Base64Regex().IsMatch(val);
        }

        [GeneratedRegex(@"^[\da-zA-Z\+/=]{8,}$")]
        private static partial Regex Base64Regex();
    }
}
