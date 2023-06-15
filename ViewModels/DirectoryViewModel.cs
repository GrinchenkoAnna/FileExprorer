using System.Collections.ObjectModel;
using System.IO;

namespace FileExplorer.ViewModels
{
    public sealed class DirectoryViewModel : FileEntityViewModel
    {
        //public ObservableCollection<DirectoryViewModel> Subfolders { get; set; }

        //public DirectoryViewModel() { }
        public DirectoryViewModel(string directoryName) : base(directoryName) { FullName = directoryName; }
        public DirectoryViewModel(DirectoryInfo directoryName) : base(directoryName.Name) { FullName = directoryName.FullName; }
    }
}