using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Plugins.Compress
{
    public class CompressDictionary
    {



        public static void Convert(string binFile, params string[] fileItems)
        {
            using var output = new FileStream(binFile, FileMode.Create);
            // output.WriteByte(3);
            var buffer = new byte[2];
            foreach (var item in fileItems)
            {
                if (!File.Exists(item))
                {
                    continue;
                }
                using var input = File.OpenRead(item);
                while (true)
                {
                    var c = input.Read(buffer, 0, 2);
                    if (c == 0)
                    {
                        break;
                    }
                    output.WriteByte((byte)((buffer[0] - 48) * 10 + buffer[1] - 48));
                }
            }
        }
    }
}
