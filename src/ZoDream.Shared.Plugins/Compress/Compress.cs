using System.IO;

namespace ZoDream.Shared.Plugins.Compress
{
    public partial class CompressStream(Stream stream, CompressDictionary dict)
    {
        private bool _withName = false;
        private bool _multiple = false;
        private bool _nextPadding = false;
        private readonly int _maxBufferLength = 4096;

        public bool IsSupport()
        {
            stream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            if (buffer[0] != 0x23 || buffer[1] != 0x5A || buffer[3] != 0x0A)
            {
                return false;
            }
            _withName = buffer[2] != 0x2;
            _multiple = buffer[2] == 0x3;
            return buffer[2] > 0x0 && buffer[2] < 0x4;
        }

    }
}
