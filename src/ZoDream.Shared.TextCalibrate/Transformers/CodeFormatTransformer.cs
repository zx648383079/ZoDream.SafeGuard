using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.TextCalibrate.Formatters;

namespace ZoDream.Shared.TextCalibrate.Transformers
{
    public class CodeFormatTransformer : IFileTransformer, IFilePreprocess
    {
        private ITextFormatter _formatter = new JsFormatter();

        public bool IsMatch(FileInfo file)
        {
            return file.Extension == ".js";
        }

        public IEnumerable<string> Preprocess(IEnumerable<string> files)
        {
            return files;
        }

        public string? Transform(string content)
        {
            return _formatter.Format(content);
        }
    }
}
