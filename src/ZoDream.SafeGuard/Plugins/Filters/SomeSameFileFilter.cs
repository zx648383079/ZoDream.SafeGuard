using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using ZoDream.SafeGuard.Finders;

namespace ZoDream.SafeGuard.Plugins.Filters
{
    public class SomeSameFileFilter : BaseFileFilter, IDisposable
    {
        public SomeSameFileFilter(IEnumerable<FileInfo> exampleItems)
        {
            foreach (var item in exampleItems)
            {
                _exampleItems.Add(new FileLoader(item));
            }
        }

        public SomeSameFileFilter(IEnumerable<string> exampleItems)
        {
            foreach (var item in exampleItems)
            {
                var info = new FileInfo(item);
                if (info.Exists)
                {
                    _exampleItems.Add(new FileLoader(info));
                    continue;
                }
                FileLoader.EachFiles(new DirectoryInfo(item), data =>
                {
                    _exampleItems.Add(new FileLoader(data));
                });
            }
        }

        protected readonly IList<FileLoader> _exampleItems = new List<FileLoader>();
        private FileEigenvalue[]? FileEigenvalues;

        public override bool IsValid(FileLoader fileInfo, CancellationToken token)
        {
            FileEigenvalues ??= _exampleItems.Select(item => new FileEigenvalue(item)).ToArray();
            var checkItems = new List<FileEigenvalue>();
            for (int i = 0; i < _exampleItems.Count; i++)
            {
                var item = _exampleItems[i];
                if (token.IsCancellationRequested)
                {
                    return false;
                }
                if (item.Length > fileInfo.Length)
                {
                    continue;
                }
                if (item.Length == fileInfo.Length)
                {
                    if (item.Md5 == fileInfo.Md5)
                    {
                        return true;
                    }
                    continue;
                }
                FileEigenvalues[i].Init();
                checkItems.Add(FileEigenvalues[i]);
            }
            if (checkItems.Count == 0)
            {
                return false;
            }
            if (token.IsCancellationRequested)
            {
                return false;
            }
            var success = false;
            EachLine(fileInfo.Reader, line =>
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }
                foreach (var item in checkItems)
                {
                    if (item.ValidLine(line))
                    {
                        success = true;
                        return false;
                    }
                }
                return true;
            });
            return success;
        }

        public void Dispose()
        {
            foreach (var item in _exampleItems)
            {
                item.Dispose();
            }
        }

        protected bool ValidFile(FileLoader example, FileLoader file)
        {

            var exampleReader = example.Reader;
            var fileReader = file.Reader;
            exampleReader.BaseStream.Seek(0, SeekOrigin.Begin);
            fileReader.BaseStream.Seek(0, SeekOrigin.Begin);
            return false;
        }

        public static void EachLine(StreamReader reader, Func<string, bool> func)
        {
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                if (func.Invoke(line))
                {
                    return;
                }
            }
        }

        public class FileEigenvalue
        {
            public string Start { get; private set; } = string.Empty;

            public string End { get; private set; }

            public string Md5 { get; private set; }

            public long Length { get; private set; }

            private bool _foundStart = false;
            private long _foundLength = 0;

            public void Init()
            {
                _foundStart = false;
                _foundLength = 0;
            }

            public bool ValidLine(string line)
            {
                if (string.IsNullOrEmpty(Start))
                {
                    return false;
                }
                if (!_foundStart)
                {
                    var i = line.IndexOf(Start);
                    if (i < 0)
                    {
                        return false;
                    }
                    _foundStart = true;
                    _foundLength = line.Length - i;
                    return false;
                }
                // TODO 做md5验算
                _foundLength += line.Length;
                if (_foundLength < Length)
                {
                    return false;
                }
                var j = line.IndexOf(End);
                if (j < 0)
                {
                    _foundStart = false;
                    return false;
                }
                return true;
            }

            public FileEigenvalue(FileLoader file) : this(file.Reader)
            {

            }

            public FileEigenvalue(StreamReader reader)
            {
                var md5 = MD5.Create();
                var lastLine = string.Empty;
                var output = new byte[1024 * 1024];
                EachLine(reader, line =>
                {
                    lastLine = line;
                    Length += line.Length;
                    if (string.IsNullOrEmpty(Start))
                    {
                        Start = line[0..10];
                    }
                    var buffer = Encoding.UTF8.GetBytes(line);
                    md5.TransformBlock(buffer, 0, buffer.Length, output, 0);
                    return true;
                });
                End = lastLine.Length > 10 ? lastLine[^10..] : lastLine;
                var res = new byte[output.Length];
                md5.TransformFinalBlock(res, 0, res.Length);
                reader.Close();
                Md5 = res.Aggregate(string.Empty, (res, b) => res += b.ToString("X2"));
            }
        }

    }
}
