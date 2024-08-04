using System;
using System.IO;
using System.Threading;

namespace ZoDream.Shared.Plugins.Transformers
{
    public class UnityRepairTransformer : BaseStreamTransformer
    {
        private readonly byte[] _tagBuffer = [0x55, 0x6E, 0x69, 0x74, 0x79];
        private long _lastMatched = -1;

        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            using var fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite);
            var count = IndexOf(fs, _tagBuffer, 30);
            if (count <= 0)
            {
                return false;
            }
            EmitFound(fileInfo);
            RemoveByte(fs, count);
            return true;
        }

        protected override bool IsValidFile(Stream stream, CancellationToken token = default)
        {
            _lastMatched = IndexOf(stream, _tagBuffer, 30);
            return _lastMatched > 0;
        }

        protected override void TranformFile(Stream stream, CancellationToken token = default)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var count = _lastMatched > 0 ? _lastMatched : IndexOf(stream, _tagBuffer, 30);
            RemoveByte(stream, count);
        }
    }
}
