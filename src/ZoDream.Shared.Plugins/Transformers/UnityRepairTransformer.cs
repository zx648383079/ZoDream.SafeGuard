using System;
using System.IO;
using System.Threading;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Plugins.Transformers
{
    public class UnityRepairTransformer : BaseStreamTransformer
    {
        private readonly byte[] _tagBuffer = [0x55, 0x6E, 0x69, 0x74, 0x79];
        private long _lastMatched = -1;

        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            if (ExtractLive2d(fileInfo, token)) 
            {
                return false;
            }
            if (!IsValidExtension(fileInfo.Extension.ToLower()))
            {
                return false;
            }
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
        /// <summary>
        /// 从拓展过滤
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private bool IsValidExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return true;
            }
            if (extension.StartsWith('.'))
            {
                extension = extension[1..];
            }
            return extension switch
            {
                "controller" or "json" or "moc3" or "meta" => false,
                _ => true
            };
        }

        private bool ExtractLive2d(FileInfo fileInfo, CancellationToken token = default)
        {
            if (fileInfo.Extension != ".asset")
            {
                return false;
            }
            using var fs = File.OpenRead(fileInfo.FullName);
            return ExtractLive2d(fileInfo, fs, token);
        }

        private bool ExtractLive2d(FileInfo fileInfo, Stream stream, CancellationToken token = default)
        {
            if (fileInfo.Extension != ".asset")
            {
                return false;
            }
            var i = fileInfo.Name.IndexOf('.');
            var name = i < 0 ? fileInfo.Name : fileInfo.Name[..i];
            var reader = LocationStorage.Reader(stream);
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }
                line = line.Trim();
                if (!line.StartsWith("_bytes:"))
                {
                    continue;
                }
                line = line[7..].Trim();
                if (!line.StartsWith("4d4f4333"))
                {
                    continue;
                }
                var output = Path.Combine(fileInfo.DirectoryName!, name + ".moc3");
                using var writer = File.OpenWrite(output);
                writer.Write(ConvertToByte(line));
                writer.Flush();
                EmitFound(new FileInfo(output));
                return true;
            }
            return false;
        }

        private byte[] ConvertToByte(string content)
        {
            var length = (int)Math.Ceiling((double)content.Length / 2);
            var buffer = new byte[length];
            for (var i = 0; i < content.Length; i+=2)
            {
                buffer[i / 2] = Convert.ToByte(content.Substring(i, 2), 16);
            }
            return buffer;
        }
    }
}
