using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace FileExplorer.ViewModels
{
    public class FileEntityViewModel 
    {
        public string Name { get; set; }

        public ObservableCollection<FileEntityViewModel> Subfolders { get; set; } 

        public string FullName { get; set; }
        public FileEntityViewModel(string name)
        {
            Name = name;
        }

        public FileEntityViewModel(DirectoryInfo directoryName)
        {
            FullName = directoryName.FullName;
        }

        public FileEntityViewModel(FileInfo fileName)
        {
            FullName = fileName.FullName;
        }
    }
}