using Avalonia.Controls;
using Avalonia.Interactivity;

using SkiaSharp;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace FileExplorer.ViewModels
{    
    public class DirectoryItemViewModel : INotifyPropertyChanged
    {
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
        
        private ObservableCollection<FileEntityViewModel> items = new();
        public ObservableCollection<FileEntityViewModel> Items
        {
            get => items;
            set
            {
                items = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
            }
        }
       
        public RootTreeNodeModel TreeRoot { get; }

        public DirectoryItemViewModel()
        {
            _history = new DirectoryHistory("Мой компьютер", "Мой компьютер");

            Name = _history.Current.DirectoryPathName;
            FilePath = _history.Current.DirectoryPath;           

            OpenCommand = new DelegateCommand(Open);            
            MoveBackCommand = new DelegateCommand(OnMoveBack, OnCanMoveBack);
            MoveForwardCommand = new DelegateCommand(OnMoveForward, OnCanMoveForward);
            MoveUpCommand = new DelegateCommand(OnMoveUp, OnCanMoveUp); //FIX!
            OpenTreeItemCommand = new DelegateCommand(OpenTreeItem);

            OpenDirectory();
            //OpenTree();

            _history.HistoryChanged += History_HistoryChanged;

            //OpenBranchCommand = new DelegateCommand(OpenBranch);
            //KeyNavigationCommand = new DelegateCommand(KeyNavigation);
            //_history.KeyPress += KeyPressed;
            
            var root = new RootTreeNodeModel()
            {
                ItemName = "Мой компьютер",
                ItemPath = "Мой компьютер",
                IsExpanded = true,
                IsDirectory = true
            };

            foreach (var logicalDrive in Directory.GetLogicalDrives())
            {
                var ch = new TreeNodeModel
                {
                    ItemName = Path.GetFullPath(logicalDrive),
                    ItemPath = logicalDrive,
                    IsExpanded = true,
                    IsDirectory = true
                };
                root.AddChild(ch);
                Populate(root, logicalDrive, 2);
            }
            root.ForceResync();
            TreeRoot = root;    
        
        }

        private void History_HistoryChanged(object? sender, EventArgs e)
        {
            MoveBackCommand?.RaiseCanExecuteChanged();
            MoveForwardCommand?.RaiseCanExecuteChanged();
        }

        #region KeyNavigation
        //public DelegateCommand OpenBranchCommand { get; }
        //public DelegateCommand KeyNavigationCommand { get; }

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
        public DelegateCommand MoveBackCommand { get; }
        public DelegateCommand MoveForwardCommand { get; }
        public DelegateCommand MoveUpCommand { get; }
        public DelegateCommand OpenTreeItemCommand { get; }

        private void Open(object parameter)
        {
            if (parameter is FileEntityViewModel directoryViewModel)
            {
                FilePath = directoryViewModel.FullName;
                Name = "Мой компьютер - " + directoryViewModel.Name;

                _history.Add(FilePath, Name);

                OpenDirectory();
            }
            else { throw new Exception(); }
        }

        void Populate(TreeNodeModel node, string itemPath, int levels)
        {
            if (levels == 0) return;

            foreach (var dir in Directory.GetDirectories(itemPath))
            {
                if (((File.GetAttributes(dir) & (FileAttributes.System | FileAttributes.Hidden))
                    != (FileAttributes.System | FileAttributes.Hidden)) && Directory.Exists(dir))
                {
                    var ch = new TreeNodeModel
                    {
                        ItemName = Path.GetFileName(dir),
                        ItemPath = dir,
                        IsExpanded = false,
                        IsDirectory = true
                    };
                    node.AddChild(ch);

                    try { Populate(ch, dir, levels - 1); }
                    catch (UnauthorizedAccessException) { }
                }
            }

            foreach (var file in Directory.GetFiles(itemPath))
            {
                if (((File.GetAttributes(file) & (FileAttributes.System | FileAttributes.Hidden))
                    != (FileAttributes.System | FileAttributes.Hidden)))
                {
                    var f = new TreeNodeModel
                    {
                        ItemName = Path.GetFileName(file),
                        ItemPath = file,
                        IsExpanded = false,
                        IsDirectory = false
                    };
                    node.AddChild(f);
                }
            }
        }

        private void OpenTreeItem(object parameter)
        {
            if (parameter is FileEntityViewModel directoryViewModel)
            {
                FilePath = directoryViewModel.FullName;
                Name = "Мой компьютер - " + directoryViewModel.Name;

                if (Name == "Мой компьютер")
                {
                    foreach (var logicalDrive in Directory.GetLogicalDrives())
                    {
                        DirectoriesAndFiles.Add(new DirectoryViewModel(logicalDrive));
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
        }

        //TODO: добавить обход защищенных папок
        private void OpenDirectory()
        {
            DirectoriesAndFiles.Clear();

            if (Name == "Мой компьютер")
            {
                foreach (var logicalDrive in Directory.GetLogicalDrives())
                {
                    DirectoriesAndFiles.Add(new DirectoryViewModel(logicalDrive));
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

        private void OnMoveUp(object obj)
        {
            _history.MoveUp();

            var current = _history.Current;
            FilePath = current.DirectoryPath;
            Name = current.DirectoryPathName;

            OpenDirectory();
        }

        private bool OnCanMoveUp(object obj) => _history.CanMoveUp;
        #endregion            
    }       
}