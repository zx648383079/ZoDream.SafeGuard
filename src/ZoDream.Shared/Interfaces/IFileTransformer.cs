using System.IO;

namespace ZoDream.Shared.Interfaces
{
    public interface IFileTransformer
    {

        public bool IsMatch(FileInfo file);

        public string? Transform(string content);
    }
}
