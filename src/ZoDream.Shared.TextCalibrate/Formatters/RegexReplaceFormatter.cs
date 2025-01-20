using System.Text.RegularExpressions;

namespace ZoDream.Shared.TextCalibrate.Formatters
{
    public class RegexReplaceFormatter(Regex regex, string replacement) : ITextFormatter
    {

        public RegexReplaceFormatter(Regex regex)
            : this (regex, string.Empty)
        {
            
        }

        public string Format(string value)
        {
            return regex.Replace(value, replacement);
        }
    }
}
