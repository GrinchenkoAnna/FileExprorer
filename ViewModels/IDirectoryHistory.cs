using Avalonia.Input;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.ViewModels
{
    internal interface IDirectoryHistory : IEnumerable<DirectoryNode>
    {
        event EventHandler HistoryChanged;
        //event EventHandler<KeyEventArgs> KeyPress;

        DirectoryNode Current { get; }
        
        bool CanMoveBack { get; }
        bool CanMoveForward { get; }

        void MoveBack();
        void MoveForward();
        void Add(string filePath, string name);
    }

    internal class DirectoryNode
    {
        public DirectoryNode PreviousNode { get; set; }
        public DirectoryNode NextNode { get; set; }
       
        public string DirectoryPath { get; }
        public string DirectoryPathName { get; }
        
        public DirectoryNode(string directoryPath, string directoryPathName)
        {
            DirectoryPath = directoryPath;
            DirectoryPathName = directoryPathName;
        }       
    }
}
