using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactions.Core;

using FileExplorer.Views;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualBasic;

using SkiaSharp;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Windows.Input;

namespace FileExplorer.ViewModels
{
    public class DirectoryItemViewModel : INotifyPropertyChanged
    {
        public class Node
        {
            public ObservableCollection<Node> Subfolders { get; set; }
            public string strNodeText { get; }
            public string strFullPath { get; }

            public Node(string _strFullPath)
            {
                strFullPath = _strFullPath;
                strNodeText = Path.GetFileName(_strFullPath);
            }
        }
        public ObservableCollection<Node> Items { get; }
        public ObservableCollection<Node> SelectedItems { get; }
        //public string strFolder { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly IDirectoryHistory _history;

        private string filePath;
        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilePath)));
            }
        }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }

        private ObservableCollection<FileEntityViewModel> directoriesAndFiles = new();
        public ObservableCollection<FileEntityViewModel> DirectoriesAndFiles
        {
            get => directoriesAndFiles;
            set
            {
                directoriesAndFiles = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirectoriesAndFiles)));
            }
        }

        private FileEntityViewModel selectFileEntity;
        public FileEntityViewModel SelectFileEntity
        {
            get => selectFileEntity;
            set
            {
                selectFileEntity = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectFileEntity)));
            }
        }

        public ObservableCollection<Node> GetSubfolders(string strPath)
        {
            ObservableCollection<Node> subfolders = new();
            string[] subdirs = Directory.GetDirectories(strPath);

            foreach (string subdir in subdirs)
            {
                Node _node = new(subdir);
                if (Directory.GetDirectories(subdir).Length > 0)
                {
                    _node.Subfolders = new ObservableCollection<Node>();
                    _node.Subfolders = GetSubfolders(subdir);
                }
                subfolders.Add(_node);
            }

            return subfolders;
        }        

        public DirectoryItemViewModel()
        {
            _history = new DirectoryHistory("Мой компьютер", "Мой компьютер");

            Name = _history.Current.DirectoryPathName;
            FilePath = _history.Current.DirectoryPath;

            //string[] strFolder = Directory.GetLogicalDrives();
            Items = new ObservableCollection<Node>();
            foreach (string strFolder in Directory.GetLogicalDrives())
            {
                Node rootNode = new Node (strFolder);
                rootNode.Subfolders = GetSubfolders(strFolder);
                Items.Add(rootNode);
            }

            OpenCommand = new DelegateCommand(Open);
            //OpenBranchCommand = new DelegateCommand(OpenBranch);
            //KeyNavigationCommand = new DelegateCommand(KeyNavigation);
            MoveBackCommand = new DelegateCommand(OnMoveBack, OnCanMoveBack);
            MoveForwardCommand = new DelegateCommand(OnMoveForward, OnCanMoveForward);            

            OpenDirectory();

            _history.HistoryChanged += History_HistoryChanged;
            //_history.KeyPress += KeyPressed;           
        }

        private void History_HistoryChanged(object? sender, EventArgs e)
        {
            MoveBackCommand?.RaiseCanExecuteChanged();
            MoveForwardCommand?.RaiseCanExecuteChanged();
        }

        #region KeyNavigation
        //private Key pressed = Key.Enter;
        //public void KeyPressed(object? sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        pressed = Key.Enter;
        //    }
        //}
        //private void KeyNavigation(object parameter)
        //{
        //    if (pressed == Key.Enter)
        //    {
        //        Open(parameter);
        //    }

        //}
        #endregion

        #region Commands
        public DelegateCommand OpenCommand { get; }
        //public DelegateCommand OpenBranchCommand { get; }
        //public DelegateCommand KeyNavigationCommand { get; }
        public DelegateCommand MoveBackCommand { get; }
        public DelegateCommand MoveForwardCommand { get; }

        private void Open(object parameter)
        {
            if (parameter is DirectoryViewModel directoryViewModel)
            {
                FilePath = directoryViewModel.FullName;
                Name = "Мой компьютер - " + directoryViewModel.Name;

                _history.Add(FilePath, Name);

                OpenDirectory();
            }
            else { throw new Exception(); }
        }

        private void OpenDirectory()
        {
            DirectoriesAndFiles.Clear();            

            if (Name == "Мой компьютер")
            {
                foreach (var logicalDrive in Directory.GetLogicalDrives()) 
                {
                    DirectoriesAndFiles.Add(new DirectoryViewModel(logicalDrive));
                    //Tree.Items.Add(DirectoryViewModel(logicalDrive));
                }
                return;
            }

            var directoryInfo = new DirectoryInfo(FilePath);

            foreach (var directory in directoryInfo.GetDirectories())
            {
                DirectoriesAndFiles.Add(new DirectoryViewModel(directory));
            }

            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                DirectoriesAndFiles.Add(new FileViewModel(fileInfo));
            }
        }
        
        private void OnMoveBack(object obj)
        {
            _history.MoveBack();

            var current = _history.Current;
            FilePath = current.DirectoryPath;
            Name = current.DirectoryPathName;

            OpenDirectory();
        }

        private bool OnCanMoveBack(object obj) => _history.CanMoveBack;

        private void OnMoveForward(object obj)
        {
            _history.MoveForward();

            var current = _history.Current;
            FilePath = current.DirectoryPath;
            Name = current.DirectoryPathName;

            OpenDirectory();
        }

        private bool OnCanMoveForward(object obj) => _history.CanMoveForward;
        #endregion        
    }       
}