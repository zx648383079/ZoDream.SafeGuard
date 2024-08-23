using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModels;
using ZoDream.Shared.Extensions;
using System.IO;
using System.Windows.Forms;
using ZoDream.Shared.Plugins.Compress;
using ZoDream.Shared.Routes;

namespace ZoDream.SafeGuard.ViewModels
{
    public class ExplorerViewModel: BindableBase, IQueryAttributable
    {

        public ExplorerViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            StartCommand = new RelayCommand(TapStart);
            SelectMatchCommand = new RelayCommand(TapSelectMatch);
            DeleteMatchCommand = new RelayCommand(TapDeleteMatch);
            StopCommand = new RelayCommand(TapStop);
            DragMatchCommand = new RelayCommand(OnDragMatch);
            SeeFileCommand = new RelayCommand(TapSeeFile);
        }

        public ITransformFinder? Finder { get; private set; }

        private int step = 0;

        public int Step {
            get => step;
            set => Set(ref step, value);
        }

        private bool isCompress;

        public bool IsCompress {
            get => isCompress;
            set => Set(ref isCompress, value);
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

        private string toolDescription = "对选择的文件及文件夹进行加压缩";

        public string ToolDescription {
            get => toolDescription;
            set => Set(ref toolDescription, value);
        }

        private string dictFileName = string.Empty;

        public string DictFileName {
            get => dictFileName;
            set => Set(ref dictFileName, value);
        }

        private string password = string.Empty;

        public string Password {
            get => password;
            set => Set(ref password, value);
        }

        private bool isCompressFolder;

        public bool IsCompressFolder {
            get => isCompressFolder;
            set => Set(ref isCompressFolder, value);
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

        public ICommand StartCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        public ICommand BackCommand { get; private set; }

        public ICommand SelectMatchCommand { get; private set; }

        public ICommand DragMatchCommand { get; private set; }

        public ICommand DeleteMatchCommand { get; private set; }


        public ICommand SeeFileCommand { get; private set; }

        private void TapSelectMatch(object? _)
        {
            var picker = new FolderBrowserDialog();
            if (picker.ShowDialog() != DialogResult.OK)
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
            
            if (!TrySetProperty())
            {
                return;
            }
            if (Finder is IFinderPreview o)
            {
                o.IsPreview = false;
            }
            Start();
        }

        private bool TrySetProperty()
        {
            if (!IsCompress)
            {
                var picker = new Microsoft.Win32.SaveFileDialog()
                {
                    RestoreDirectory = true,
                    FileName = "dict.bin"
                };
                if (picker.ShowDialog() != true)
                {
                    return false;
                }
                (Finder as DictionaryTransformer).OutputFileName = picker.FileName;
                return true;
            }
            if (string.IsNullOrEmpty(DictFileName))
            {
                MessageBox.Show("请选择字典文件");
                return false;
            }
            if (Finder is IFinderCompress o)
            {
                var folder = new FolderBrowserDialog()
                {
                    ShowNewFolderButton = true
                };
                if (folder.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }
                o.OutputFolder = folder.SelectedPath;
                o.Password = Password;
                o.DictionaryFileName = DictFileName;
                o.Multiple = IsCompressFolder;
            }
            return true;
        }

        private void Start()
        {
            if (Finder is null)
            {
                return;
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
            }
            else if (arg is FileTransformItem t)
            {
                fileName = t.FileName;
            }
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            if (!File.Exists(fileName))
            {
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
                IsCompress = transformer is not DictionaryTransformer;
                if (transformer is ITransformFinder finder)
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
