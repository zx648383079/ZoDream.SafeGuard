using System.Collections.Generic;
using System.Threading;
using ZoDream.Shared.Finders;

namespace ZoDream.Shared.Finders.Filters
{
    public class ExtensionFileFilter : BaseFileFilter
    {
        public const string IMAGE_FLAGS = ".gif;.jpg;.jpeg;.png;.bmp;.webp";
        public const string MEDIA_FLAGS = ".flv;.swf;.mkv;.avi;.rm;.rmvb;.mpeg;.mpg;.ogg;.ogv;.mov;.wmv;.mp4;.webm;.mp3;.wav;.mid;.flac;.ape";
        public const string DOCUMENT_FLAGS = ".txt;.pdf;.doc;.docx;.xls;.xlsx;.ppt;.pptx";
        public const string ZIPARCHIVE_FLAGS = ".zip;.rar;.7z";

        public ExtensionFileFilter()
        {

        }
        public ExtensionFileFilter(string extension)
        {
            Add(extension);
        }

        public ExtensionFileFilter(IEnumerable<string> items)
        {
            Add(items);
        }

        private readonly IList<string> _extensionItems = new List<string>();

        public void Add(string extension)
        {
            Add(extension.Split(';'));
        }
        public void Add(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                _extensionItems.Add(item.Trim().ToLower());
            }
        }

        public override bool IsValid(FileLoader fileInfo, CancellationToken token)
        {
            return _extensionItems.Count == 0 || _extensionItems.Contains(fileInfo.File.Extension.ToLower());
        }
    }
}
