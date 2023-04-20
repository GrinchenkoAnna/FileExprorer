using System.ComponentModel;

namespace FileExplorer.ViewModels
{
    public abstract class FileEntityViewModel
    {
        public string Name { get; }

        //public string FullName { get; set; }
        protected FileEntityViewModel(string name)
        {
            Name = name;
        }

        //: INotifyPropertyChanged
        //public event PropertyChangedEventHandler? PropertyChanged;
    }
}