using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZoDream.SafeGuard.Plugins.Transformers
{
    public partial class ReplaceTransformer : IFileTransformer
    {
        public bool IsMatch(FileInfo file)
        {
            return file.Extension == ".htm" || file.Extension == ".html";
        }

        public string? Transform(string content)
        {
            return MatchRegex().Replace(content, "");
        }

        [GeneratedRegex(@"\<!--\s+Copyright[\s\S]+Spidersoft\s+Ltd\s+--\>")]
        private static partial Regex MatchRegex();
    }
}
