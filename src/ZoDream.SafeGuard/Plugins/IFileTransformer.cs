using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.SafeGuard.Plugins
{
    public interface IFileTransformer
    {

        public bool IsMatch(FileInfo file);

        public string? Transform(string content);
    }
}
