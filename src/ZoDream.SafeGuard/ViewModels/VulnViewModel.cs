using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.SafeGuard.Extensions;
using ZoDream.SafeGuard.Models;
using ZoDream.Shared.ViewModel;

namespace ZoDream.SafeGuard.ViewModels
{
    public class VulnViewModel: BindableBase
    {

        public VulnViewModel()
        {
            BackCommand = new RelayCommand(TapBack); 
            StartCommand = new RelayCommand(TapStart);
            StopCommand = new RelayCommand(TapStop);
            SelectMatchCommand = new RelayCommand(TapSelectMatch);
            DeleteMatchCommand = new RelayCommand(TapDeleteMatch);
            DragMatchCommand = new RelayCommand(OnDragMatch);
            BlackStepCommand = new RelayCommand(TapBlackStep);
            WhiteStepCommand = new RelayCommand(TapWhiteStep);
            LogStepCommand = new RelayCommand(TapLogStep);
        }


        private int scanType;

        public int ScanType {
            get => scanType;
            set => Set(ref scanType, value);
        }


        private int step;

        public int Step {
            get => step;
            set => Set(ref step, value);
        }

        private bool isPaused = true;

        public bool IsPaused {
            get => isPaused;
            set {
                Set(ref isPaused, value);
                StopText = value ? "重新开始" : "停止";
            }
        }

        private string progressTip = string.Empty;

        public string ProgressTip {
            get => progressTip;
            set => Set(ref progressTip, value);
        }

        private string stopText = "停止";

        public string StopText {
            get => stopText;
            set => Set(ref stopText, value);
        }

        private string baseUri = string.Empty;

        public string BaseUri {
            get => baseUri;
            set => Set(ref baseUri, value);
        }

        private ObservableCollection<FileInfoItem> matchFileItems = new();

        public ObservableCollection<FileInfoItem> MatchFileItems {
            get => matchFileItems;
            set => Set(ref matchFileItems, value);
        }
        #region 绑定的方法
        public ICommand BackCommand { get; private set; }
        public ICommand BlackStepCommand { get; private set; }
        public ICommand WhiteStepCommand { get; private set; }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public ICommand SelectMatchCommand { get; private set; }

        public ICommand DragMatchCommand { get; private set; }
        public ICommand DeleteMatchCommand { get; private set; }
        public ICommand LogStepCommand { get; private set; }
        #endregion

        private void TapStart(object? _)
        {
            Step = 3;
            IsPaused = false;
        }

        private void TapStop(object? _)
        {
            IsPaused = true;
        }

        private void TapBack(object? _)
        {
            Step = 0;
        }

        private void TapBlackStep(object? _)
        {
            ScanType = 0;
            Step = 1;
        }

        private void TapWhiteStep(object? _)
        {
            ScanType = 1;
            Step = 2;
        }

        private void TapSelectMatch(object? _)
        {
            var picker = new System.Windows.Forms.FolderBrowserDialog();
            if (picker.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            MatchFileItems.TryAdd(picker.SelectedPath);
        }

        private void OnDragMatch(object? items)
        {
            if (items is IEnumerable<string> o)
            {
                MatchFileItems.TryAdd(o);
            }
        }

        private void TapDeleteMatch(object? arg)
        {
            if (arg is FileInfoItem item)
            {
                MatchFileItems.Remove(item);
            }
        }

        public void TapLogStep(object? arg)
        {
            Step = 3;
        }
    }
}
