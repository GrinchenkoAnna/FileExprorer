using ReactiveUI;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FileExplorer.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler? PropertyChanged;

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

        private DirectoryItemViewModel currentDirectoryItem = new();
        public DirectoryItemViewModel CurrentDirectoryItem
        {
            get => currentDirectoryItem;
            set
            {
                currentDirectoryItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentDirectoryItem)));
            }
        }

        private DirectoryItemViewModel treeDirectoryItem = new();
        public DirectoryItemViewModel TreeDirectoryItem
        {
            get => treeDirectoryItem;
            set
            {
                treeDirectoryItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentDirectoryItem)));
            }
        }

        public MainWindowViewModel()
        {
            DirectoryItems.Add(new DirectoryItemViewModel());
        }        
    }
}