using System.ComponentModel;

namespace FileExplorer.ViewModels
{    
    public abstract class FileEntityViewModel : MainWindowViewModel
    {
       
        public string Name { get; }

        //public string FullName { get; set; }
        protected FileEntityViewModel(string name)
        {
            Name = name;
        }
    }    
}