using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface IMLPredict
    {
        public FileCheckStatus Predict(string fileName, string text);
    }
}
