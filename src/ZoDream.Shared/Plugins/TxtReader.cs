using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Plugins
{
    internal class TxtReader: IDisposable
    {

        public TxtReader(string fileName)
        {
            FileName = fileName;
            BaseStream = File.OpenRead(fileName);
            Encoding = TxtEncoder.GetEncoding(BaseStream);
        }

        public FileStream BaseStream { get; private set; }

        public Encoding Encoding { get; private set; }

        public long Position 
        { 
            get { return BaseStream.Position; }
            set { Seek(value); }
        }

        public string FileName { get; private set; }

        public int LineNumber { get; set; }

        public int ColumnNumber { get; set; }

        public void Seek(long position)
        {
            BaseStream.Seek(position, SeekOrigin.Begin);
        }

        public int ReadByte()
        {
            try
            {
                return BaseStream.ReadByte();
            }
            catch (IndexOutOfRangeException)
            {
                return -1;
            }
        }

        public string? ReadLine()
        {
            var bytes = new List<byte>();
            var isEnd = false;
            int bInt;
            while (true)
            {
                bInt = ReadByte();
                if (bInt == -1)
                {
                    isEnd = true;
                    break;
                }
                if (bInt == 0x0A)
                {
                    break;
                }
                if (bInt == 0x0D)
                {
                    var p = Position;
                    var next = ReadByte();
                    if (next == 0x0A)
                    {
                        break;
                    }
                    if (next == -1)
                    {
                        isEnd = true;
                        break;
                    }
                    Seek(p);
                    break;
                }
                bytes.Add(Convert.ToByte(bInt));
            }
            if (bytes.Count == 0)
            {
                return isEnd ? null : string.Empty;
            }
            return Encoding.GetString([.. bytes]);
        }

        public char ReadChar()
        {
            return '.';
        }

        public void Dispose()
        {
            BaseStream.Dispose();
        }
    }
}
