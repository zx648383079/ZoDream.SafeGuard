using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZoDream.SafeGuard.ViewModels;

namespace ZoDream.SafeGuard.Pages
{
    /// <summary>
    /// ScanPage.xaml 的交互逻辑
    /// </summary>
    public partial class ScanPage : Page
    {
        public ScanPage()
        {
            InitializeComponent();
        }

        public ScanViewModel ViewModel => (ScanViewModel)DataContext;


        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // ViewModel.SeeFileCommand.Execute((sender as ListViewItem)?.DataContext);
        }
    }
}
