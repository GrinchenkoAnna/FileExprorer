using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FileExplorer.ViewModels
{
    public sealed class DirectoryViewModel : FileEntityViewModel
    {
        //public ObservableCollection<DirectoryViewModel> Subfolders { get; set; }
        public DirectoryViewModel() { }
        public DirectoryViewModel(string directoryName) : base(directoryName) { FullName = directoryName; }
        public DirectoryViewModel(DirectoryInfo directoryName) : base(directoryName.Name) 
        { 
            FullName = directoryName.FullName;
            if (Directory.GetLogicalDrives().Contains(FullName))
            {
                Type = "Локальный диск";
                IsSystemFolder = true;
            }
            DateOfChange = directoryName.LastWriteTime.ToShortDateString() + " " + directoryName.LastWriteTime.ToShortTimeString();
            if (directoryName.GetType().ToString() == "System.IO.DirectoryInfo")
            {
                Type = "Папка с файлами";
            }
        }       
    }
}