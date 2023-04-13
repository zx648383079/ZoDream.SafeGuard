using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.SafeGuard.Extensions;
using ZoDream.SafeGuard.Finders;
using ZoDream.SafeGuard.Models;
using ZoDream.Shared.ViewModel;
using ZoDream.Shared.ViewModels;

namespace ZoDream.SafeGuard.ViewModels
{
    public class ScanViewModel : BindableBase
    {

        public ScanViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            FileStepCommand = new RelayCommand(TapFileStep);
            TextStepCommand = new RelayCommand(TapTextStep);
            AiStepCommand = new RelayCommand(TapAiStep);
            StartCommand = new RelayCommand(TapStart);
            SelectExampleCommand = new RelayCommand(TapSelectExample);
            SelectMatchCommand = new RelayCommand(TapSelectMatch);
            DeleteExampleCommand = new RelayCommand(TapDeleteExample);
            DeleteMatchCommand = new RelayCommand(TapDeleteMatch);
            StopCommand = new RelayCommand(TapStop);
            DragExampleCommand = new RelayCommand(OnDragExample);
            DragMatchCommand = new RelayCommand(OnDragMatch);
            Finder = new StorageFinder();
            Finder.Finished += Finder_Finished;
            Finder.FileChanged += Finder_FileChanged;
            Finder.FoundChanged += Finder_FoundChanged;
        }

        public StorageFinder Finder { get; private set; }

        #region 绑定的属性

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

        private string[] fileMatchTypes = new string[]{ "完全匹配", "仅包含"};

        public string[] FileMatchTypes {
            get => fileMatchTypes;
            set => Set(ref fileMatchTypes, value);
        }

        private int fileMatchType;

        public int FileMatchType {
            get => fileMatchType;
            set => Set(ref fileMatchType, value);
        }

        private string fileNameRegex = string.Empty;

        public string FileNameRegex {
            get => fileNameRegex;
            set => Set(ref fileNameRegex, value);
        }

        private string[] exampleTextTypes = new string[] {"文本匹配", "关键字匹配", "正则匹配" };

        public string[] ExampleTextTypes {
            get => exampleTextTypes;
            set => Set(ref exampleTextTypes, value);
        }

        private int exampleTextType;

        public int ExampleTextType {
            get => exampleTextType;
            set => Set(ref exampleTextType, value);
        }




        private string exampleText = string.Empty;

        public string ExampleText {
            get => exampleText;
            set => Set(ref exampleText, value);
        }



        private ObservableCollection<FileInfoItem> exampleItems = new();

        public ObservableCollection<FileInfoItem> ExampleItems {
            get => exampleItems;
            set => Set(ref exampleItems, value);
        }


        private ObservableCollection<FileInfoItem> matchFileItems = new();

        public ObservableCollection<FileInfoItem> MatchFileItems {
            get => matchFileItems;
            set => Set(ref matchFileItems, value);
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


        #endregion

        #region 绑定的方法
        public ICommand BackCommand { get; private set; }
        public ICommand FileStepCommand { get; private set; }
        public ICommand TextStepCommand { get; private set; }
        public ICommand AiStepCommand { get; private set; }

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public ICommand SelectExampleCommand { get; private set; }

        public ICommand DragExampleCommand { get; private set; }
        public ICommand SelectMatchCommand { get; private set; }

        public ICommand DragMatchCommand { get; private set; }
        public ICommand DeleteExampleCommand { get; private set; }
        public ICommand DeleteMatchCommand { get; private set; }

        private void TapSelectExample(object? _)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择样本文件",
                RestoreDirectory = true,
                Multiselect = true,
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            ExampleItems.TryAdd(picker.FileNames);
        }

        private void OnDragExample(object? items)
        {
            if (items is IEnumerable<string> o)
            {
                ExampleItems.TryAdd(o);
            }
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

        private void TapDeleteExample(object? arg)
        {
            if (arg is FileInfoItem item)
            {
                ExampleItems.Remove(item);
            }
        }

        private void TapDeleteMatch(object? arg)
        {
            if (arg is FileInfoItem item)
            {
                MatchFileItems.Remove(item);
            }
        }

        private void TapStart(object? _)
        {
            Step = 4;
            IsPaused = false;
            Finder.Start(MatchFileItems.Select(i => i.FileName).ToArray());
        }

        private void TapStop(object? _)
        {
            Finder.Stop();
            IsPaused = true;
        }

        private void TapBack(object? _)
        {
            Step = 0;
        }

        private void TapFileStep(object? _)
        {
            ScanType = 0;
            Step = 1;
        }

        private void TapTextStep(object? _)
        {
            ScanType = 1;
            Step = 2;
        }

        private void TapAiStep(object? _)
        {
            ScanType = 2;
            Step = 3;
        }

        private void TapSeeFile(object? arg)
        {
            if (arg is FileInfoItem o)
            {
                Process.Start("explorer", $"/select,{o.FileName}");
            }
        }

        #endregion


        private void Finder_FoundChanged(FileInfo item)
        {
            App.Current.Dispatcher.Invoke(() => {
                
            });
        }

        private void Finder_FileChanged(string fileName)
        {
            App.Current.Dispatcher.Invoke(() => {
                ProgressTip = fileName;
            });
        }

        private void Finder_Finished()
        {
            App.Current.Dispatcher.Invoke(() => {
                ProgressTip = string.Empty;
                IsPaused = true;
            });
        }

    }
}
