namespace ZoDream.Shared.TextCalibrate.Formatters
{
    public class ReplaceFormatter(string search, string replacement) : ITextFormatter
    {

        public ReplaceFormatter(string search)
            : this (search, string.Empty)
        {
            
        }

        public string Format(string value)
        {
            return value.Replace(search, replacement);
        }
    }
}
