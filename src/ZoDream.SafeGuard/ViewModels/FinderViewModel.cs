using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using ZoDream.Shared.Extensions;
using ZoDream.Shared.Finders;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.Storage;
using ZoDream.Shared.ViewModels;

namespace ZoDream.SafeGuard.ViewModels
{
    public class FinderViewModel: BindableBase, IQueryAttributable
    {
        public FinderViewModel()
        {
            TestCommand = new RelayCommand(TapTest);
            TestExecuteCommand = new RelayCommand(TapTestExecute);
            DragTestCommand = new RelayCommand(TapDragTest);
            BackCommand = new RelayCommand(TapBack);
            StartCommand = new RelayCommand(TapStart);
            SelectMatchCommand = new RelayCommand(TapSelectMatch);
            DeleteMatchCommand = new RelayCommand(TapDeleteMatch);
            StopCommand = new RelayCommand(TapStop);
            DragMatchCommand = new RelayCommand(OnDragMatch);
            SeeFileCommand = new RelayCommand(TapSeeFile);
            SelectOutputCommand = new RelayCommand(TapSelectOutput);
        }
        public ITransformFinder? Finder { get; private set; }

        private int step = 0;

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

        private string toolName = "批量修改";

        public string ToolName {
            get => toolName;
            set => Set(ref toolName, value);
        }

        private string toolDescription = "对选择的文件及文件夹进行查找，并根据需要修改文件的内容/或文件名";

        public string ToolDescription {
            get => toolDescription;
            set => Set(ref toolDescription, value);
        }

        private string outputFolder = string.Empty;

        public string OutputFolder 
        {
            get => outputFolder;
            set => Set(ref outputFolder, value);
        }



        private ObservableCollection<FileInfoItem> matchFileItems = [];

        public ObservableCollection<FileInfoItem> MatchFileItems {
            get => matchFileItems;
            set => Set(ref matchFileItems, value);
        }

        private ObservableCollection<FileTransformItem> transformItems = [];

        public ObservableCollection<FileTransformItem> TransformItems {
            get => transformItems;
            set => Set(ref transformItems, value);
        }


        private string testText = string.Empty;

        public string TestText {
            get => testText;
            set => Set(ref testText, value);
        }

        private string testResult = string.Empty;

        public string TestResult {
            get => testResult;
            set => Set(ref testResult, value);
        }


        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public ICommand BackCommand { get; private set; }

        public ICommand SelectMatchCommand { get; private set; }
        public ICommand SelectOutputCommand { get; private set; }

        public ICommand DragMatchCommand { get; private set; }

        public ICommand DeleteMatchCommand { get; private set; }
        public ICommand TestCommand { get; private set; }
        public ICommand TestExecuteCommand { get; private set; }

        public ICommand DragTestCommand { get; private set; }

        public ICommand SeeFileCommand { get; private set; }


        private void TapSelectOutput(object? _)
        {
            var picker = new System.Windows.Forms.FolderBrowserDialog()
            {
                ShowNewFolderButton = true,
            };
            if (picker.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            OutputFolder = picker.SelectedPath;
        }

        private void TapTest(object? _)
        {
            if (Finder is IFinderPreview o)
            {
                o.IsPreview = true;
                Start();
            } else
            {
                Step = 2;
            }
            
        }

        private async void TapDragTest(object? items)
        {
            TestText = string.Empty;
            if (items is IEnumerable<string> o)
            {
                foreach (var item in o)
                {
                    if (!File.Exists(item))
                    {
                        continue;
                    }
                    TestText = await LocationStorage.ReadAsync(item);
                }
            }
        }

        private void TapTestExecute(object? _)
        {
            TestResult = Finder!.Transform(TestText);
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

        private void TapStart(object? _)
        {
            if (Finder is IFinderPreview o)
            {
                o.IsPreview = false;
            }
            Start();
        }

        private void Start()
        {
            if (Finder is null)
            {
                return;
            }
            if (Finder is IFinderOutput o)
            {
                o.OutputFolder = OutputFolder;
            }
            Step = 1;
            IsPaused = false;
            TransformItems.Clear();
            Finder.Start(MatchFileItems.Select(i => i.FileName).ToArray());
        }

        private void TapStop(object? _)
        {
            Finder?.Stop();
            IsPaused = true;
        }

        private void TapBack(object? _)
        {
            Step = 0;
        }

        private void TapSeeFile(object? arg)
        {
            var fileName = string.Empty;
            if (arg is FileInfoItem o)
            {
                fileName = o.FileName;
            } else if (arg is FileTransformItem t)
            {
                fileName = t.FileName;
            }
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            if (!File.Exists(fileName)) {
                fileName = Path.GetDirectoryName(fileName);
            }
            Process.Start("explorer", $"/select,{fileName}");
        }


        private void Finder_FoundChanged(FileInfoItem item)
        {
            var isPreview = Finder is IFinderPreview o && o.IsPreview;
            App.Current.Dispatcher.Invoke(() => {
                TransformItems.Add(new FileTransformItem(item,
                    isPreview ? FileTransformStatus.Waiting : FileTransformStatus.Done
                ));
            });
        }

        private void Finder_FileChanged(string fileName)
        {
            App.Current.Dispatcher.Invoke(() => {
                ProgressTip = fileName;
            });
        }

        private void Finder_FileProgress(string fileName, long current, long total)
        {
            if (total <= 0)
            {
                return;
            }
            App.Current.Dispatcher.Invoke(() => {
                ProgressTip = $"[{current * 100 / total}%]{fileName}";
            });
        }

        private void Finder_Finished()
        {
            App.Current.Dispatcher.Invoke(() => {
                ProgressTip = string.Empty;
                IsPaused = true;
            });
        }

        public void ApplyQueryAttributes(IDictionary<string, object> queries)
        {
            if (queries.TryGetValue("tool", out var tool) && tool is ToolItem o 
                && o.Target is not null)
            {
                ToolName = o.Title;
                if (!string.IsNullOrWhiteSpace(o.Description))
                {
                    ToolDescription = o.Description;
                }
                var transformer = Activator.CreateInstance(o.Target);
                if (transformer is IFileTransformer target)
                {
                    Finder = new TransformFinder();
                    Finder.Finished += Finder_Finished;
                    Finder.FileChanged += Finder_FileChanged;
                    Finder.FoundChanged += Finder_FoundChanged;
                    Finder.Clear();
                    Finder.Add(
                        target
                    );
                } else if (transformer is ITransformFinder finder)
                {
                    Finder = finder;
                    Finder.Finished += Finder_Finished;
                    Finder.FileChanged += Finder_FileChanged;
                    Finder.FoundChanged += Finder_FoundChanged;
                }
                if (transformer is IFinderProgress pro)
                {
                    pro.FileProgress += Finder_FileProgress;
                }
                Step = 0;
                MatchFileItems.Clear();
                TransformItems.Clear();
            }
            
        }

        
    }
}
