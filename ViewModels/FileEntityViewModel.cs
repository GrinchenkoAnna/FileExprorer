using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
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
        public bool IsSystemFolder { get; set; }
        
        [JsonIgnore]
        public bool IsRoot { get; set; }

        [JsonIgnore]
        public int NumberOfItems { get; set; }

        [JsonIgnore]
        public ObservableCollection<FileEntityViewModel> Subfolders { get; set; }

        public FileEntityViewModel(string name) { Name = name; }

        public FileEntityViewModel() { }

        public FileEntityViewModel(DirectoryInfo directoryName) { FullName = directoryName.FullName; }

        public FileEntityViewModel(FileInfo fileName) { FullName = fileName.FullName; }

        protected string ConvertValue(long value)
        {
            string[] units = new string[5] { " КБ", " МБ", " ГБ", " ТБ", " ПБ" };
            int i = 0;
            long size = value / 1024;

            while (size > 999 && i < 3)
            {
                size /= 1024;
                i++;
            }

            return (size).ToString() + units[i];
        }
    }
}