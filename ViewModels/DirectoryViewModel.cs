using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileExplorer.ViewModels
{
    public sealed class DirectoryViewModel : FileEntityViewModel
    {
        //public ObservableCollection<DirectoryViewModel> Subfolders { get; set; }
        public DirectoryViewModel() { }
        public DirectoryViewModel(string directoryName) : base(directoryName) 
        { 
            FullName = directoryName;

            //логические диски
            if (Directory.GetLogicalDrives().Contains(FullName))
            {
                Type = "Локальный диск";
                IsSystemFolder = true;
                IsRoot = true;
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.Name == FullName)
                    {
                        Size = ConvertValue(drive.AvailableFreeSpace) + " свободно из " + ConvertValue(drive.TotalSize);
                    }
                }                
            }

            //остальное
            else
            {
                IsRoot = false;
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
                if (directoryInfo.Attributes.ToString().Contains("System"))
                {
                    Type = "Системная папка";
                    IsSystemFolder = true;
                }
                else
                {
                    Type = "Папка с файлами";
                    IsSystemFolder = false;
                }
                DateOfChange = directoryInfo.LastWriteTime.ToShortDateString() + " " + directoryInfo.LastWriteTime.ToShortTimeString();
            }            
        }
        
        public DirectoryViewModel(DirectoryInfo directoryName) : base(directoryName.Name)
        {
            FullName = directoryName.FullName;
            if (directoryName.Attributes.ToString().Contains("System"))
            {
                Type = "Системная папка";
                IsSystemFolder = true;
            }
            else
            {
                Type = "Папка с файлами";
                IsSystemFolder = false;
            }
            DateOfChange = directoryName.LastWriteTime.ToShortDateString() + " " + directoryName.LastWriteTime.ToShortTimeString();
            //Size = DirectoryItemViewModel.GetDirectorySize(directoryName.FullName).ToString();                
        }
        
        //private static long DirectorySize(DirectoryInfo directoryInfo)
        //{
        //    long size = 0;
        //    FileInfo[] files = directoryInfo.GetFiles();
        //    foreach (FileInfo file in files)
        //    {
        //        size += file.Length;
        //    }

        //    DirectoryInfo[] dirs = directoryInfo.GetDirectories();
        //    foreach (var dir in dirs)
        //    {
        //        size += DirectorySize(dir);
        //    }

        //    return size;
        //}
    }
}