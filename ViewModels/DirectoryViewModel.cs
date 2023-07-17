using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FileExplorer.ViewModels
{
    public sealed class DirectoryViewModel : FileEntityViewModel
    {
        //public ObservableCollection<DirectoryViewModel> Subfolders { get; set; }
        public DirectoryViewModel() { }
        public DirectoryViewModel(string directoryName) : base(directoryName) 
        { 
            FullName = directoryName;
            if (Directory.GetLogicalDrives().Contains(FullName))
            {
                Type = "Локальный диск";
                IsSystemFolder = true;
                NumberOfItems = 0;
            }           
        }
        public DirectoryViewModel(DirectoryInfo directoryName) : base(directoryName.Name)
        {
            FullName = directoryName.FullName;
            if (directoryName.Attributes.ToString().Contains("System"))
            {
                Type = "Системная папка";
                IsSystemFolder = true;
            }
            else
            {
                Type = "Папка с файлами";
                IsSystemFolder = false;
            }
            DateOfChange = directoryName.LastWriteTime.ToShortDateString() + " " + directoryName.LastWriteTime.ToShortTimeString();
        }
    }
}