using System.Collections.ObjectModel;
using System.IO;

namespace FileExplorer.ViewModels
{
    public sealed class FileViewModel : FileEntityViewModel
    {
        public FileViewModel() { }
        public FileViewModel(string name) : base(name) { }
        //public ObservableCollection<FileViewModel> Subfolders { get; set; }

        public FileViewModel(FileInfo fileInfo) : base(fileInfo.Name) { FullName = fileInfo.FullName; }
    }
}