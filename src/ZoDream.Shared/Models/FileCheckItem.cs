using System.IO;
using ZoDream.Shared.ViewModels;

namespace ZoDream.Shared.Models
{
    public class FileCheckItem : BindableBase
    {

        public string Name { get; set; }

        public string FileName { get; set; }

        private FileCheckStatus status;

        public FileCheckStatus Status {
            get => status;
            set {
                Set(ref status, value);
                if (string.IsNullOrWhiteSpace(Message))
                {
                    Message = status switch
                    {
                        FileCheckStatus.Waiting => "队列中",
                        FileCheckStatus.Checking => "检测中",
                        FileCheckStatus.Normal => "未检测到",
                        FileCheckStatus.Valid => "检测到",
                        FileCheckStatus.Poisoning => "中毒",
                        FileCheckStatus.Virus => "病毒",
                        FileCheckStatus.Pass => "跳过",
                        _ => string.Empty,
                    };
                }
            }
        }

        private string message = string.Empty;

        public string Message {
            get => message;
            set => Set(ref message, value);
        }

        public FileCheckItem(string fileName)
        {
            FileName = fileName;
            Name = Path.GetFileName(fileName);
        }

        public FileCheckItem(string name, string fileName, FileCheckStatus status)
        {
            Name = name;
            FileName = fileName;
            Status = status;
        }
    }
}
