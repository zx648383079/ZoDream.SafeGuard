using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.Routes;
using ZoDream.Shared.ViewModels;

namespace ZoDream.SafeGuard.ViewModels
{
    public class MainViewModel: BindableBase
    {

        public MainViewModel()
        {
            HomeCommand = new RelayCommand(TapHome);
            ScanCommand = new RelayCommand(TapScan);
            TrainCommand = new RelayCommand(TapTrain);
            SettingCommand = new RelayCommand(TapSetting);
            VulnCommand = new RelayCommand(TapVuln);
            ToolCommand = new RelayCommand(TapTool);
        }

        private bool isHome = true;

        public bool IsHome {
            get => isHome;
            set => Set(ref isHome, value);
        }

        public ICommand HomeCommand { get; private set; }
        public ICommand ScanCommand { get; private set; }
        public ICommand TrainCommand { get; private set; }
        public ICommand ToolCommand { get; private set; }
        public ICommand SettingCommand { get; private set; }

        public ICommand VulnCommand { get; private set; }

        private void TapHome(object? _)
        {
            IsHome = true;
        }

        private void TapVuln(object? _)
        {
            IsHome = false;
            ShellManager.GoToAsync("vuln");
        }

        private void TapTool(object? _)
        {
            IsHome = false;
            ShellManager.GoToAsync("tool");
        }

        private void TapScan(object? _)
        {
            IsHome = false;
            ShellManager.GoToAsync("scan");
        }

        private void TapTrain(object? _)
        {
            IsHome = false;
            ShellManager.GoToAsync("train");
        }

        private void TapSetting(object? _)
        {
            IsHome = false;
            ShellManager.GoToAsync("setting");
        }

    }
}
