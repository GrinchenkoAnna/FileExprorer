using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileExplorer.ViewModels
{
    public sealed class FileViewModel : FileEntityViewModel
    {
        public FileViewModel() { }
        public FileViewModel(string name) : base(name) { }
        //public ObservableCollection<FileViewModel> Subfolders { get; set; }

        public FileViewModel(FileInfo fileInfo) : base(fileInfo.Name) 
        { 
            FullName = fileInfo.FullName;
            DateOfChange = fileInfo.LastWriteTime.ToShortDateString() + " " + fileInfo.LastWriteTime.ToShortTimeString();
            if (fileInfo.GetType().ToString() == "System.IO.FileInfo")
            {
                Type = "Файл";
            }
            Size = (fileInfo.Length/1024).ToString() + " КБ";
        }
    }
}