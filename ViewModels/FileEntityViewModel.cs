using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json.Serialization;

namespace FileExplorer.ViewModels
{
    public class FileEntityViewModel 
    {
        [JsonInclude]
        public string Name { get; set; }
        [JsonIgnore]
        public ObservableCollection<FileEntityViewModel> Subfolders { get; set; }
        [JsonInclude]
        public string FullName { get; set; }

        public FileEntityViewModel(string name)
        {
            Name = name;
        }

        //public FileEntityViewModel() { }

        public FileEntityViewModel(DirectoryInfo directoryName)
        {
            FullName = directoryName.FullName;
        }

        public FileEntityViewModel(FileInfo fileName)
        {
            FullName = fileName.FullName;
        }

        public FileEntityViewModel() { }
    }
}