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

        [JsonInclude]
        public string FullName { get; set; }

        [JsonIgnore]
        public string DateOfChange { get; set; }

        [JsonIgnore]
        public string DateOfCreation { get; set; }

        [JsonIgnore]
        public string Type { get; set; }

        [JsonIgnore]
        public string Size { get; set; }

        [JsonIgnore]
        public int NumberOfItems { get; set; }

        [JsonIgnore]
        public bool IsSystemFolder { get; set; }

        [JsonIgnore]
        public ObservableCollection<FileEntityViewModel> Subfolders { get; set; }        

        public FileEntityViewModel(string name)
        {
            Name = name;
        }

        public FileEntityViewModel() { }

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