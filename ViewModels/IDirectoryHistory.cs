using Avalonia.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        bool CanMoveUp { get; }
        bool CanDelete { get; }
        bool CanReplace { get; }

        void MoveBack();
        void MoveForward();
        void MoveUp();
        void Add(string filePath, string name);
    }

    internal class DirectoryNode
    {
        public DirectoryNode PreviousNode { get; set; }
        public DirectoryNode NextNode { get; set; }
        public DirectoryNode UpNode { get; set; }


        public string DirectoryPath { get; }
        public string DirectoryPathName { get; }

        public DirectoryNode(string directoryPath, string directoryPathName)
        {
            DirectoryPath = directoryPath;
            DirectoryPathName = directoryPathName;
        }

        public DirectoryNode(string directoryPathName)
        {          
            DirectoryPathName = directoryPathName;
        }
    }
}
