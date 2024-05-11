using System.IO;

namespace ZoDream.SafeGuard.Plugins
{
    public interface IFileTransformer
    {

        public bool IsMatch(FileInfo file);

        public string? Transform(string content);
    }
}
