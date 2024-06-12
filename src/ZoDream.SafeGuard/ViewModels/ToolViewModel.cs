using System.Collections.ObjectModel;
using ZoDream.Shared.CodeScanner.Transformers;
using ZoDream.Shared.Models;
using ZoDream.Shared.Plugins.Transformers;
using ZoDream.Shared.TextCalibrate.Transformers;
using ZoDream.Shared.ViewModels;

namespace ZoDream.SafeGuard.ViewModels
{
    public class ToolViewModel : BindableBase
    {

        public ToolViewModel() 
        {
            Items.Add(new ToolGroupItem("常用")
            {
                Items = [ 
                    new ToolItem("文件批量修改", "\uE932", "tool/finder")
                    {
                        Target = typeof(PHPTypeTransformer)
                    },
                    new ToolItem("文本批量替换", "\uE14B", "tool/finder")
                    {
                        Target = typeof(ReplaceTransformer)
                    },
                    new ToolItem("TXT精校", "\uE77C", "tool/finder")
                    {
                        Target = typeof(TxtCalibrateTransformer)
                    },
                    new ToolItem("安全文件名", "\uEC77", "tool/finder")
                    {
                        Target = typeof(NasRenameTransformer)
                    },
                    new ToolItem("文件重命名", "\uE8AC", "tool/rename")
                    {
                        Target = typeof(NasRenameTransformer)
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
