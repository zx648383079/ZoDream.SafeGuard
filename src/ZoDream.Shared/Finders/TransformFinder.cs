using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Finders
{
    public partial class TransformFinder : StorageFinder, ITransformFinder
    {

        public event FinderEventHandler? FoundChanged;
        public IList<IFileTransformer> TransformerItems { get; set; } = [];

        public void Clear()
        {
            TransformerItems.Clear();
        }

        public void Add(IFileTransformer transformer)
        {
            TransformerItems.Add(transformer);
        }

        protected override IEnumerable<string> Preprocess(IEnumerable<string> files, CancellationToken token = default)
        {
            foreach (var item in TransformerItems)
            {
                if (item is IFilePreprocess target)
                {
                    files = target.Preprocess(files);
                }
            }
            return files;
        }

        protected override bool IsValidFile(FileInfo fileInfo, CancellationToken token = default)
        {
            if (TransformerItems == null || TransformerItems.Count == 0)
            {
                return false;
            }
            foreach (var item in TransformerItems)
            {
                if (token.IsCancellationRequested)
                {
                    return false;
                }
                if (item.IsMatch(fileInfo))
                {
                    return true;
                }
            }
            return false;
        }

        protected override Task ProcessFile(FileInfo fileInfo, CancellationToken token = default)
        {
            if (!fileInfo.Exists || TransformerItems == null || TransformerItems.Count == 0)
            {
                return Task.CompletedTask;
            }
            FoundChanged?.Invoke(new Models.FileInfoItem(fileInfo));
            var fs = new FileStream(fileInfo.FullName, FileMode.OpenOrCreate);
            var content = ReadText(fs, IsCharsetFile(fileInfo), out var encoding);
            var isUpdated = false;
            foreach (var item in TransformerItems)
            {
                if (token.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }
                if (!item.IsMatch(fileInfo))
                {
                    continue;
                }
                var res = item.Transform(content);
                if (res is null)
                {
                    continue;
                }
                content = res;
                isUpdated = true;
            }
            if (isUpdated)
            {
                fs.Seek(0, SeekOrigin.Begin);
                using var writer = new StreamWriter(fs, encoding);
                writer.Write(content);
                writer.Flush();
                fs.SetLength(fs.Position);
            }
            return Task.CompletedTask;
        }

        protected string ReadText(FileStream fs, out Encoding encoding)
        {
            return ReadText(fs, false, out encoding);
        }

        /// <summary>
        /// 判断文件是否包含 charset 信息
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected virtual bool IsCharsetFile(FileInfo file)
        {
            var ext = file.Extension[1..].ToLower();
            return ext == "htm" || ext == "html";
        }
        /// <summary>
        /// 获取内容，同时返回编码
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="isMatchCharset"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        protected virtual string ReadText(FileStream fs, bool isMatchCharset, out Encoding encoding)
        {
            encoding = TxtEncoder.GetEncoding(fs);
            var reader = new StreamReader(fs, encoding);
            var content = reader.ReadToEnd();
            if (!isMatchCharset)
            {
                return content;
            }
            var charsetMatch = CharsetRegex().Match(content);
            if (charsetMatch is null || !charsetMatch.Success)
            {
                return content;
            }
            encoding = Encoding.GetEncoding(charsetMatch.Groups["charset"].Value);
            fs.Seek(0, SeekOrigin.Begin);
            reader = new StreamReader(fs, encoding);
            return reader.ReadToEnd();
        }
        
        public string Transform(string content, CancellationToken token = default)
        {
            if (TransformerItems == null || TransformerItems.Count == 0)
            {
                return content;
            }
            foreach (var item in TransformerItems)
            {
                if (token.IsCancellationRequested)
                {
                    return content;
                }
                var res = item.Transform(content);
                if (res is null)
                {
                    continue;
                }
                content = res;
            }
            return content;
        }

        [GeneratedRegex("charset\\b\\s*=\\s*\"*(?<charset>[^\"]*)")]
        private static partial Regex CharsetRegex();
    }
}
