using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.Shared.Extensions;
using ZoDream.Shared.ViewModels;

namespace ZoDream.SafeGuard.ViewModels
{
    public class RenameViewModel: BindableBase
    {
        public RenameViewModel()
        {
            DragFileCommand = new RelayCommand(OnDragFile);
            AddFileCommand = new RelayCommand(TapAddFile);
            MoveDownCommand = new RelayCommand(TapMoveDown);
            MoveLastCommand = new RelayCommand(TapMoveLast);
            MoveTopCommand = new RelayCommand(TapMoveTop);
            MoveUpCommand = new RelayCommand(TapMoveUp);
            OrderCommand = new RelayCommand(TapOrder);
            RemoveCommand = new RelayCommand(TapRemove);
            StartCommand = new RelayCommand(TapStart);
            ClearCommand = new RelayCommand(TapClear);
        }

        private CancellationTokenSource? _cancelTokenSource;
        private int _sortType = -1;
        private bool _sortDescending = false;

        private bool _isPaused = true;

        public bool IsPaused {
            get => _isPaused;
            set {
                Set(ref _isPaused, value);
                BtnText = value ? "开始" : "停止";
            }
        }

        private string _btnText = "开始";

        public string BtnText {
            get => _btnText;
            set => Set(ref _btnText, value);
        }


        private string[] _replaceMatchType = ["文件名", "拓展名", "文件名和拓展名"];

        public string[] ReplaceMatchType {
            get => _replaceMatchType;
            set => Set(ref _replaceMatchType, value);
        }

        private string[] _replaceMatchRule = ["字符替换", "正则替换", "单字符替换"];

        public string[] ReplaceMatchRule {
            get => _replaceMatchRule;
            set => Set(ref _replaceMatchRule, value);
        }

        private int _replaceTypeIndex;

        public int ReplaceTypeIndex {
            get => _replaceTypeIndex;
            set => Set(ref _replaceTypeIndex, value);
        }

        private int _replaceRuleIndex;

        public int ReplaceRuleIndex {
            get => _replaceRuleIndex;
            set {
                Set(ref _replaceRuleIndex, value);
                if (value == 2 && string.IsNullOrWhiteSpace(ReplaceMatch))
                {
                    ReplaceMatch = "@#$\\/'\"|:;*?<>";
                }
            }
        }

        private string _replaceMatch = string.Empty;

        public string ReplaceMatch {
            get => _replaceMatch;
            set {
                Set(ref _replaceMatch, value);
                RefreshRename();
            }
        }

        private string _replaceValue = string.Empty;

        public string ReplaceValue {
            get => _replaceValue;
            set {
                Set(ref _replaceValue, value);
                RefreshRename();
            }
        }

        private string _orderRule = string.Empty;

        public string OrderRule {
            get => _orderRule;
            set {
                Set(ref _orderRule, value);
                RefreshRename();
            }
        }

        private int _orderPad = 2;

        public int OrderPad {
            get => _orderPad;
            set {
                Set(ref _orderPad, value);
                RefreshRename();
            }
        }

        private int _orderBegin = 1;

        public int OrderBegin {
            get => _orderBegin;
            set {
                Set(ref _orderBegin, value);
                RefreshRename();
            }
        }

        private bool _extensionUpper;

        public bool ExtensionUpper {
            get => _extensionUpper;
            set {
                Set(ref _extensionUpper, value);
                RefreshRename();
            }
        }


        private ObservableCollection<RenameFileItemViewModel> fileItems = [];

        public ObservableCollection<RenameFileItemViewModel> FileItems {
            get => fileItems;
            set => Set(ref fileItems, value);
        }

        private RenameFileItemViewModel? _fileSelectedItem;

        public RenameFileItemViewModel? FileSelectedItem {
            get => _fileSelectedItem;
            set => Set(ref _fileSelectedItem, value);
        }


        public ICommand DragFileCommand {  get; private set; }
        public ICommand StartCommand {  get; private set; }
        public ICommand MoveTopCommand {  get; private set; }
        public ICommand MoveLastCommand {  get; private set; }
        public ICommand MoveUpCommand {  get; private set; }
        public ICommand MoveDownCommand {  get; private set; }
        public ICommand RemoveCommand {  get; private set; }
        public ICommand ClearCommand {  get; private set; }
        public ICommand OrderCommand {  get; private set; }
        public ICommand AddFileCommand {  get; private set; }

        private void OnDragFile(object? arg)
        {
            if (arg is IEnumerable<string> items)
            {
                AddFile(items);
            }
        }


        private void TapAddFile(object? _)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择文件",
                RestoreDirectory = true,
                Multiselect = true,
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            AddFile(picker.FileNames);
        }

        private void TapMoveTop(object? arg)
        {
            var item = arg is RenameFileItemViewModel o ? o : FileSelectedItem;
            if (item is null)
            {
                return;
            }
            FileItems.MoveToFirst(FileItems.IndexOf(item));
            RefreshOrder();
        }
        private void TapMoveLast(object? arg)
        {
            var item = arg is RenameFileItemViewModel o ? o : FileSelectedItem;
            if (item is null || FileItems.IndexOf(item) == FileItems.Count - 1)
            {
                return;
            }
            FileItems.MoveToLast(FileItems.IndexOf(item));
            RefreshOrder();
        }
        private void TapMoveUp(object? arg)
        {
            var item = arg is RenameFileItemViewModel o ? o : FileSelectedItem;
            if (item is null)
            {
                return;
            }
            FileItems.MoveUp(FileItems.IndexOf(item));
            RefreshOrder();
        }
        private void TapMoveDown(object? arg)
        {
            var item = arg is RenameFileItemViewModel o ? o : FileSelectedItem;
            if (item is null)
            {
                return;
            }
            FileItems.MoveDown(FileItems.IndexOf(item));
            RefreshOrder();
        }

        private void TapOrder(object? arg)
        {
            if (int.TryParse(arg?.ToString(), out var i))
            {
                if (i == _sortType)
                {
                    _sortDescending = !_sortDescending;
                    Sort();
                }
                else
                {
                    _sortType = i;
                    _sortDescending = false;
                    Sort();
                }
            }
        }
        private void TapRemove(object? arg)
        {
            var item = arg is RenameFileItemViewModel o ? o : FileSelectedItem;
            if (item is null) {
                return;
            }
            FileItems.Remove(item);
            RefreshOrder();
        }
        private void TapClear(object? _)
        {
            FileItems.Clear();
            _sortType = -1;
        }
        private void TapStart(object? _)
        {
            if (!IsPaused)
            {
                TapStop();
                return;
            }
            _cancelTokenSource = new CancellationTokenSource();
            var token = _cancelTokenSource.Token;
            IsPaused = false;
            Task.Factory.StartNew(() => {
                foreach (var item in FileItems)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    if (item.ReplaceName == item.Name)
                    {
                        continue;
                    }
                    var fileName = Path.Combine(Path.GetDirectoryName(item.FileName)!, item.ReplaceName);
                    if (string.IsNullOrEmpty(fileName)) 
                    {
                        continue;
                    }
                    try
                    {
                        if (item.IsFolder)
                        {
                            Directory.Move(item.FileName, fileName);
                        }
                        else
                        {
                            File.Move(item.FileName, fileName);
                        }
                        item.FileName = fileName;
                        item.Name = item.ReplaceName;
                        item.Extension = Path.GetExtension(item.FileName);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                IsPaused = true;
            }, token);
        }

        private void TapStop()
        {
            _cancelTokenSource?.Cancel();
            IsPaused = true;
        }

        public void Sort()
        {
            switch (_sortType)
            {
                case 9:
                    Sort(item => {
                        var name = item.NameWithoutExtension;
                        var items = Regex.Matches(name, @"\d+");
                        if (items.Count == 0)
                        {
                            return name;
                        }
                        return string.Join(string.Empty, items.Select(i => i.Value));
                    }, _sortDescending);
                    break;
                case 2:
                    Sort(item => item.Extension, _sortDescending);
                    break;
                case 1:
                    Sort(item => item.CreatedAt, _sortDescending);
                    break;
                default:
                    Sort(item => item.Name, _sortDescending);
                    break;
            }
        }

        public void Sort<TKey>(Func<RenameFileItemViewModel, TKey> keySelector, bool descending)
        {
            var items = !descending ? [.. FileItems.OrderBy(keySelector)] : FileItems.OrderByDescending(keySelector).ToArray();
            FileItems.Clear();
            var i = 0;
            foreach (var item in items)
            {
                item.Index = ++i;
                FileItems.Add(item);
            }
            if (OrderRule.Contains("{no}", StringComparison.OrdinalIgnoreCase))
            {
                RefreshRename();
            }
        }

        public void RefreshRename()
        {
            foreach (var item in FileItems)
            {
                RefreshRename(item);
            }
        }

        private void RefreshRename(RenameFileItemViewModel item)
        {
            var (name, extension) = UseReplace(item);
            if (!string.IsNullOrWhiteSpace(extension))
            {
                extension = ExtensionUpper ? extension.ToUpper() : extension.ToLower();
            }
            name = UseOrderNo(name, item.Index);
            item.ReplaceName = name + extension;
        }

        private string UseOrderNo(string name, int index)
        {
            if (!OrderRule.Contains('{'))
            {
                return name;
            }
            var no = (index + OrderBegin - 1).ToString().PadLeft(OrderPad, '0');
            return OrderRule.Replace("{no}", no, StringComparison.OrdinalIgnoreCase)
                .Replace("{name}", name, StringComparison.OrdinalIgnoreCase);
        }

        private (string, string) UseReplace(RenameFileItemViewModel item)
        {
            if (string.IsNullOrWhiteSpace(ReplaceMatch))
            {
                return (item.NameWithoutExtension, item.Extension);
            }
            switch (ReplaceTypeIndex)
            {
                case 2:
                    var name = UseReplace(item.Name);
                    if (item.IsFolder)
                    {
                        return (name, string.Empty);
                    }
                    var i = name.LastIndexOf('.');
                    if (i == -1)
                    {
                        return (name, string.Empty);
                    }
                    return (name[..i], name[i..]);
                case 1:
                    return (item.NameWithoutExtension, UseReplace(item.Extension));
                default:
                    return (UseReplace(item.NameWithoutExtension), item.Extension);
            }
        }

        private string UseReplace(string name)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(ReplaceMatch))
            {
                return name;
            }
            try
            {
                switch (ReplaceRuleIndex)
                {
                    case 2:
                        var end = ReplaceValue.Length > 0 ?
                            ReplaceValue[^1..] : string.Empty;
                        for (var i = 0; i < ReplaceMatch.Length; i++)
                        {
                            name = name.Replace(ReplaceMatch.Substring(i, 1), 
                                ReplaceValue.Length > i ? 
                                ReplaceValue.Substring(i, 1) : end);
                        }
                        return name;
                    case 1:
                        return Regex.Replace(name, ReplaceMatch, ReplaceValue);
                    default:
                        return name.Replace(ReplaceMatch, ReplaceValue);
                }
            }
            catch (Exception)
            {
                return name;
            }
        }

        public void RefreshOrder()
        {
            for (int i = 0; i < FileItems.Count; i++)
            {
                FileItems[i].Index = i + 1;
            }
            if (OrderRule.Contains("{no}", StringComparison.OrdinalIgnoreCase))
            {
                RefreshRename();
            }
        }

        public void AddFile(IEnumerable<string> items)
        {
            var i = FileItems.Count;
            foreach (var item in items)
            {
                var target = new RenameFileItemViewModel(item)
                {
                    Index = ++i
                };
                RefreshRename(target);
                FileItems.Add(target);
            }
        }
    }
}
