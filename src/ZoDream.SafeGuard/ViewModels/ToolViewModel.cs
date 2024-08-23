using System.Collections.ObjectModel;
using ZoDream.Shared.CodeScanner.Transformers;
using ZoDream.Shared.Models;
using ZoDream.Shared.Plugins.Compress;
using ZoDream.Shared.Plugins.Transformers;
using ZoDream.Shared.TextCalibrate.Transformers;
using ZoDream.Shared.ViewModels;

namespace ZoDream.SafeGuard.ViewModels
{
    public class ToolViewModel : BindableBase
    {

        public ToolViewModel() 
        {
            Items.Add(new("常用")
            {
                Items = [ 
                    new("文件批量修改", "\uE932", "tool/finder")
                    {
                        Target = typeof(PHPTypeTransformer)
                    },
                    new("文本批量替换", "\uE14B", "tool/finder")
                    {
                        Target = typeof(ReplaceTransformer)
                    },
                    new("TXT精校", "\uE77C", "tool/finder")
                    {
                        Target = typeof(TxtCalibrateTransformer)
                    },
                    new("安全文件名", "\uEC77", "tool/finder")
                    {
                        Target = typeof(NasRenameTransformer)
                    },
                    new("文件重命名", "\uE8AC", "tool/rename")
                    {
                        Target = typeof(NasRenameTransformer)
                    },
                    new("文件移动", "\uE8DE", "tool/finder")
                    {
                        Target = typeof(CopyFileTransformer)
                    },
                    new("文件删除", "\uE74D", "tool/finder")
                    {
                        Description = "删除指定文件和删除空文件夹",
                        Target = typeof(DeleteFileTransformer)
                    },
                ]
            });
            Items.Add(new("文件")
            {
                Items = [
                    new("Unity文件修复", "\uE90F", "tool/finder")
                    {
                        Target = typeof(UnityRepairTransformer)
                    },
                    new("Tar文件修复", "\uE8E5", "tool/finder")
                    {
                        Target = typeof(TarRepairTransformer)
                    },
                    new("Base64文件解码", "\uE72E", "tool/finder")
                    {
                        Target = typeof(Base64Transformer)
                    },
                ]
            });
            Items.Add(new("解压缩")
            {
                Items = [
                    new("压缩字典", "\uE7F1", "tool/explorer")
                    {
                        Target = typeof(DictionaryTransformer)
                    },
                    new("压缩文件", "\uE81E", "tool/explorer")
                    {
                        Target = typeof(InflateTransformer)
                    },
                    new("解压文件", "\uE8C8", "tool/explorer")
                    {
                        Target = typeof(DeflateTransformer)
                    },
                ]
            });
        }


        private ObservableCollection<ToolGroupItem> items = [];

        public ObservableCollection<ToolGroupItem> Items {
            get => items;
            set => Set(ref items, value);
        }


    }
}
