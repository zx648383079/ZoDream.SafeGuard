using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZoDream.SafeGuard.Models;
using ZoDream.Shared.ViewModel;
using ZoDream.Shared.ViewModels;
using ZoDream.SafeGuard.Extensions;

namespace ZoDream.SafeGuard.ViewModels
{
    public class TrainViewModel: BindableBase
    {

        public TrainViewModel()
        {
            PlayCommand = new RelayCommand(TapPlay);
            StopCommand = new RelayCommand(TapStop);
            ResetCommand = new RelayCommand(TapReset);
            SelectNormalCommand = new RelayCommand(TapSelectNormal);
            SelectPoisoningCommand = new RelayCommand(TapSelectPoisoning);
            SelectVirusCommand = new RelayCommand(TapSelectVirus);
            DeleteNormalCommand = new RelayCommand(TapDeleteNormal);
            DeletePoisoningCommand = new RelayCommand (TapDeletePoisoning);
            DeleteVirusCommand = new RelayCommand(TapDeleteVirus);
            DragNormalCommand = new RelayCommand(OnDragNormal);
            DragPoisoningCommand = new RelayCommand(OnDragPoisoning);
            DragVirusCommand = new RelayCommand(OnDragVirus);
        }

        private bool isPaused = true;

        public bool IsPaused {
            get => isPaused;
            set {
                Set(ref isPaused, value);
            }
        }

        private ObservableCollection<FileInfoItem> normalFileItems = new();

        public ObservableCollection<FileInfoItem> NormalFileItems {
            get => normalFileItems;
            set => Set(ref normalFileItems, value);
        }

        private ObservableCollection<FileInfoItem> poisoningFileItems = new();

        public ObservableCollection<FileInfoItem> PoisoningFileItems {
            get => poisoningFileItems;
            set => Set(ref poisoningFileItems, value);
        }

        private ObservableCollection<FileInfoItem> virusFileItems = new();

        public ObservableCollection<FileInfoItem> VirusFileItems {
            get => virusFileItems;
            set => Set(ref virusFileItems, value);
        }

        public ICommand PlayCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }

        public ICommand SelectNormalCommand { get; private set; }

        public ICommand DragNormalCommand { get; private set; }
        public ICommand DeleteNormalCommand { get; private set; }
        public ICommand SelectPoisoningCommand { get; private set; }

        public ICommand DragPoisoningCommand { get; private set; }
        public ICommand DeletePoisoningCommand { get; private set; }
        public ICommand SelectVirusCommand { get; private set; }

        public ICommand DragVirusCommand { get; private set; }
        public ICommand DeleteVirusCommand { get; private set; }

        private void TapPlay(object? _)
        {

        }

        private void TapStop(object? _)
        {

        }

        private void TapReset(object? _)
        {

        }

        private void TapSelectNormal(object? _)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择正常文件",
                RestoreDirectory = true,
                Multiselect = true,
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            NormalFileItems.TryAdd(picker.FileNames);
        }

        private void OnDragNormal(object? items)
        {
            if (items is IEnumerable<string> o)
            {
                NormalFileItems.TryAdd(o);
            }
        }

        private void TapDeleteNormal(object? arg)
        {
            if (arg is FileInfoItem item)
            {
                NormalFileItems.Remove(item);
            }
        }

        private void TapSelectPoisoning(object? _)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择中毒文件",
                RestoreDirectory = true,
                Multiselect = true,
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            PoisoningFileItems.TryAdd(picker.FileNames);
        }

        private void OnDragPoisoning(object? items)
        {
            if (items is IEnumerable<string> o)
            {
                PoisoningFileItems.TryAdd(o);
            }
        }

        private void TapDeletePoisoning(object? arg)
        {
            if (arg is FileInfoItem item)
            {
                PoisoningFileItems.Remove(item);
            }

        }

        private void TapSelectVirus(object? _)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Title = "选择病毒文件",
                RestoreDirectory = true,
                Multiselect = true,
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            VirusFileItems.TryAdd(picker.FileNames);
        }

        private void OnDragVirus(object? items)
        {
            if (items is IEnumerable<string> o)
            {
                VirusFileItems.TryAdd(o);
            }
        }

        private void TapDeleteVirus(object? arg)
        {
            if (arg is FileInfoItem item)
            {
                VirusFileItems.Remove(item);
            }
        }

    }
}
