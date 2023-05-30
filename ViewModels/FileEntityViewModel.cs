using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FileExplorer.ViewModels
{
    public class FileEntityViewModel //abstract
    {
        public string Name { get; }
        public ObservableCollection<FileEntityViewModel> Subfolders { get; set; } //added

        public string FullName { get; set; }
        public FileEntityViewModel(string name)
        {
            Name = name;
        }       
    }
}