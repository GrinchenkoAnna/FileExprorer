using Avalonia.Controls.Selection;
using Avalonia.Xaml.Interactions.Draggable;
using FileExplorer.ViewModels;
using FileExplorer.ViewModels.Pages;

using JetBrains.Annotations;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

using static FileExplorer.ViewModels.DirectoryItemViewModel;

namespace FileExplorer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected ISynchronizationHelper _synchronizationHelper;

        private string path;
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref path, value);
            }
        }

        #region Main Panel
        private ObservableCollection<DirectoryItemViewModel> directoryItems = new();
        public ObservableCollection<DirectoryItemViewModel> DirectoryItems
        {
            get => directoryItems;
            set
            {
                directoryItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirectoryItems)));
            }
        }

        private DirectoryItemViewModel currentDirectoryItem;
        public DirectoryItemViewModel CurrentDirectoryItem
        {
            get => currentDirectoryItem;
            set
            {
                currentDirectoryItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentDirectoryItem)));
            }
        }     
        #endregion
                

        public MainWindowViewModel(ISynchronizationHelper synchronizationHelper)
        {
            _synchronizationHelper = synchronizationHelper;

            var vm = new DirectoryItemViewModel(_synchronizationHelper);

            DirectoryItems.Add(vm);
            CurrentDirectoryItem = vm;
        }
    }
}