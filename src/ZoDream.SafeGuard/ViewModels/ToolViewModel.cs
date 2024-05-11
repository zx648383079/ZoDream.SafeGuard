using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.SafeGuard.Models;
using ZoDream.SafeGuard.Plugins.Transformers;
using ZoDream.Shared.ViewModel;

namespace ZoDream.SafeGuard.ViewModels
{
    public class ToolViewModel : BindableBase
    {

        public ToolViewModel() 
        {
            Items.Add(new ToolGroupItem("常用")
            {
                Items = [ 
                    new ToolItem("文件批量修改", "\uE13E", "tool/finder")
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
                ]
            });
        }


        private ObservableCollection<ToolGroupItem> items = new();

        public ObservableCollection<ToolGroupItem> Items {
            get => items;
            set => Set(ref items, value);
        }


    }
}
