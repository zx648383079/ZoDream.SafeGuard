using System;
using System.IO;
using System.Threading;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Plugins.Filters
{
    public class MLFileFilter(IMLPredict nn) : BaseFileFilter
    {
        public override bool IsValid(FileLoader fileInfo, CancellationToken token)
        {
            var status = nn.Predict(fileInfo.File.Name, fileInfo.Reader.ReadToEnd());
            VaildStatus = status;
            return status > Models.FileCheckStatus.Normal;
        }
    }
}
