using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface IFileProvider
    {

        public Task<FileInfoItem> GetItemsAsync(string folder);
        public Task<FileInfoItem> GetItemsAsync(FileInfoItem folder);

        public Task EachFileAsync(string folder, Action<FileInfoItem> success, CancellationToken token = default);
        public Task EachFileAsync(FileInfoItem folder, Action<FileInfoItem> success, CancellationToken token = default);

        public Task<string> ReadAsync(string fileName);
        public Task<string> ReadAsync(FileInfoItem file);

        public Task<Stream> GetStreamAsync(string fileName);
        public Task<Stream> GetStreamAsync(FileInfoItem file);

        public Task<bool> DeleteAsync(string fileName);
        public Task<bool> DeleteAsync(FileInfoItem file);
    }
}
