using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace FileExplorer.ViewModels
{
    public sealed class FileViewModel : FileEntityViewModel
    {
        public FileViewModel() { }
        public FileViewModel(string name) : base(name) { }
        //public ObservableCollection<FileViewModel> Subfolders { get; set; }

        public FileViewModel(FileInfo fileInfo) : base(fileInfo.Name) 
        { 
            FullName = fileInfo.FullName;
            DateOfChange = fileInfo.LastWriteTime.ToShortDateString() + " " + fileInfo.LastWriteTime.ToShortTimeString();
            DateOfCreation = fileInfo.CreationTime.ToShortDateString() + " " + fileInfo.CreationTime.ToShortTimeString();
            if (fileInfo.GetType().ToString() == "System.IO.FileInfo")
            {
                Type = GetFileType(fileInfo);
                IsSystemFolder = false;
            }
            Size = GetFileSize(fileInfo);            
            NumberOfItems = 0;
        }
        
        private string GetFileSize(FileInfo fileInfo)
        {
            string[] units = new string[5]{" КБ", " МБ", " ГБ", " ТБ", " ПБ" };
            int i = 0;
            long size = fileInfo.Length / 1024;

            while (size > 999 && i < 3)
            {
                size /= 1024;
                i++;
            }

            return (size).ToString() + units[i];
        }
        
        private static string GetFileType(FileInfo fileInfo)
        {
            string extention = Path.GetExtension(fileInfo.FullName);
            string result = extention switch
            {
                ".txt" or ".log" or ".gitattributes" or ".gitignore" => "Текстовый документ",
                ".rtf" => "Формат RTF",
                ".ico" => "Значок",
                ".docx" or ".doc" => "Документ Microsoft Word",
                ".pptx" => "Презентация Microsoft PowerPoint",
                ".csv" => "Файл Microsoft Excel",
                ".json" or ".bin" => extention.Substring(1).ToUpperInvariant() + " file",
                ".ini" => "Параметры конфигурации",
                ".vsix" => "Microsoft Visual Studio Extention",
                ".sln" => "Microsoft Visual Studio Solution",
                ".cpp" => "C++ source file",
                ".cs" => "C# Source File",
                ".csproj" => "C# Project File",
                ".torrent" => "Торрент файл",
                ".7z" => "7z Archive",
                ".exe" => "Приложение",
                ".dll" => "Расширение приложения",
                _ => "Файл " + extention.Substring(1).ToUpperInvariant(),
            };
            return result;
        }
    }
}