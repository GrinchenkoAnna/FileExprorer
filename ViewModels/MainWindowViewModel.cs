using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace FileExplorer.ViewModels
{
    public partial class MainWindowViewModel : INotifyPropertyChanged
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

        private string filePath;
        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilePath)));
            }
        }

        private ObservableCollection<FileEntityViewModel> directoriesAndFiles = new();
        public ObservableCollection<FileEntityViewModel> DirectoriesAndFiles
        {
            get => directoriesAndFiles;
            set
            {
                directoriesAndFiles = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirectoriesAndFiles)));
            }
        } 

        //public ICommand OpenCommand { get; }

        public MainWindowViewModel()
        {
            //OpenCommand = new DelegateCommand(Open);

            foreach (var logicalDrive in Directory.GetLogicalDrives())
            {
                //DirectoriesAndFiles.Add(logicalDrive);
                DirectoriesAndFiles.Add(new DirectoryViewModel(logicalDrive));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //private void Open(object parameter)
        //{
        //    if (parameter is DirectoryViewModel directoryViewModel)
        //    {
        //        FilePath = directoryViewModel.FullName;
        //        DirectoriesAndFiles.Clear();

        //        var directoryInfo = new DirectoryInfo(FilePath);

        //        foreach (var directory in directoryInfo.GetDirectories())
        //        {
        //            DirectoriesAndFiles.Add(new DirectoryViewModel(directory));
        //        }

        //        foreach (var fileInfo in directoryInfo.GetFiles())
        //        {
        //            DirectoriesAndFiles.Add(new FileViewModel(fileInfo));
        //        }
        //    }
        //}       
    }
}