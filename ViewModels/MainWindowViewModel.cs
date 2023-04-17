using System;
using System.ComponentModel;

namespace FileExplorer.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string mainDiskName;

        public string MainDiskName 
        { 
            get => mainDiskName;
            set
            {
                mainDiskName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MainDiskName)));
            }
        }

        public MainWindowViewModel()
        {
            MainDiskName = Environment.SystemDirectory;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}