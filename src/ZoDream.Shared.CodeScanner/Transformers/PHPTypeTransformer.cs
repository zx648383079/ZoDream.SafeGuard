using System.IO;
using System.Text.RegularExpressions;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.CodeScanner.Transformers
{
    /// <summary>
    /// PHP 给方法添加返回类型
    /// </summary>
    public partial class PHPTypeTransformer : IFileTransformer
    {
        public bool IsMatch(FileInfo file)
        {
            if (file.Extension != ".php")
            {
                return false;
            }
            var name = file.Name[..^file.Extension.Length];
            var prefixItems = new string[] { "Tables" };
            foreach (var item in prefixItems)
            {
                if (name.EndsWith(item))
                {
                    return true;
                }
            }
            return false;
        }

        public string? Transform(string content)
        {
            return TableRegex().Replace(content, match => {
                if (match.Groups[3].Value == ":")
                {
                    return match.Value;
                }
                var type = "void";//match.Groups[2].Value == "tableName" ? "string" : "array";
                return $"{match.Groups[1].Value}: {type} ";
            });
        }

        [GeneratedRegex(@"(function\s+(up)\(\))\s*(:?)")]
        private static partial Regex TableRegex();
    }
}
