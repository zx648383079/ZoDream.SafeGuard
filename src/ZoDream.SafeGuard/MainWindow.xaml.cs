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
using ZoDream.SafeGuard.Pages;
using ZoDream.Shared.Routes;

namespace ZoDream.SafeGuard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShellManager.RegisterRoute("scan", typeof(ScanPage));
            ShellManager.RegisterRoute("train", typeof(TrainPage));
            ShellManager.RegisterRoute("vuln", typeof(VulnerabilityPage));
            ShellManager.RegisterRoute("tool", typeof(ToolPage));
            ShellManager.RegisterRoute("tool/finder", typeof(FinderPage));
            ShellManager.RegisterRoute("tool/rename", typeof(RenamePage));
            ShellManager.RegisterRoute("setting", typeof(SettingPage));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShellManager.BindFrame(BodyPanel);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            ShellManager.UnBind();
        }
    }
}
