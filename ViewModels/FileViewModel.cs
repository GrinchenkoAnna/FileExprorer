using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

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
            DateOfCreation = fileInfo.CreationTime.ToShortDateString() + " " + fileInfo.CreationTime.ToShortTimeString();
            if (fileInfo.GetType().ToString() == "System.IO.FileInfo")
            {
                Type = "Файл";
                IsSystemFolder = false;
            }
            Size = GetFileSize(fileInfo);            
            NumberOfItems = 0;
        }
        
        private string GetFileSize(FileInfo fileInfo)
        {
            string[] units = new string[5]{" КБ", " МБ", " ГБ", " ТБ", " ПБ" };
            int i = 0;
            long size = fileInfo.Length / 1024;

            while (size > 999 && i < 3)
            {
                size = size / 1024;
                i++;
            }

            return (size).ToString() + units[i];
        }
    }
}