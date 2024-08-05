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
        public CompressDictionary(Stream stream)
        {
            _reader = stream;
        }
        public CompressDictionary(string binFile)
        {
            _reader = File.OpenRead(binFile);
        }

        private readonly Stream _reader;
        private readonly byte[] _buffer = new byte[2];
        private long _postion = 0;

        public byte ReadByte()
        {
            var code = _reader.ReadByte();
            if (code < 0)
            {
                _reader.Seek(0, SeekOrigin.Begin);
            }
             code = _reader.ReadByte();
            return (byte)code;
            //var i = _postion % 2;
            //if (i == 0)
            //{
            //    LoadByte();
            //}
            //_postion++;
            //return _buffer[i];
        }

        //private void LoadByte()
        //{
        //    var code = _reader.ReadByte();
        //    if (code < 0)
        //    {
        //        _reader.Seek(0, SeekOrigin.Begin);
        //    }
        //    code = _reader.ReadByte();
        //    _buffer[0] = (byte)(code / 10);
        //    _buffer[1] = (byte)(code % 10);
        //}

        public void WriteByte(byte[] buffer, int length) 
        {
            var data = new byte[(int)Math.Ceiling((double)length / 2)];
            var j = 0;
            for (var i = 0; i < length; i += 2)
            {
                j = i / 2;
                data[j] = (byte)((buffer[i] - 48) * 10 +
                    (i + 1 >= length ? 0 : (buffer[i + 1] - 48)));
            }
            _reader.Write(buffer, 0, data.Length);
        }

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
