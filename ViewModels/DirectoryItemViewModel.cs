using Avalonia.Controls;
using Avalonia.Interactivity;

using SkiaSharp;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

        private ObservableCollection<FileEntityViewModel> quickAccessItems = new();
        public ObservableCollection<FileEntityViewModel> QuickAccessItems
        {
            get => quickAccessItems;
            set
            {
                quickAccessItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuickAccessItems)));
            }
        }                

        public DirectoryItemViewModel()
        {
            _history = new DirectoryHistory("Мой компьютер", "Мой компьютер");

            Name = _history.Current.DirectoryPathName;
            FilePath = _history.Current.DirectoryPath;           

            OpenCommand = new DelegateCommand(Open);
            AddToQuickAccessCommand = new DelegateCommand(AddToQuickAccess);
            DeleteCommand = new DelegateCommand(Delete, OnCanDelete);
            RenameCommand = new DelegateCommand(Rename);
            MoveBackCommand = new DelegateCommand(OnMoveBack, OnCanMoveBack);
            MoveForwardCommand = new DelegateCommand(OnMoveForward, OnCanMoveForward);
            MoveForwardCommand = new DelegateCommand(OnMoveUp, OnCanMoveUp);

            OpenDirectory();
            _ = OpenTree();
            //CreateTree();

            _history.HistoryChanged += History_HistoryChanged;
            QuickAccessItems = new ObservableCollection<FileEntityViewModel>();

            //OpenBranchCommand = new DelegateCommand(OpenBranch);
            //KeyNavigationCommand = new DelegateCommand(KeyNavigation);
            //_history.KeyPress += KeyPressed;           
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
        public DelegateCommand AddToQuickAccessCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand RenameCommand { get; }
        public DelegateCommand MoveBackCommand { get; }
        public DelegateCommand MoveForwardCommand { get; }
        public DelegateCommand MoveUpCommand { get; }

        #region OpenDirectory
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
            
            //catch (FileNotFoundException) { }
            //catch (DirectoryNotFoundException) { }
        }
        #endregion

        #region Delete
        private void Delete(object parameter)
        {
            if (parameter is FileEntityViewModel item)
            {
                FilePath = item.FullName;
                if (item is DirectoryViewModel) 
                    Directory.Delete(FilePath);
                else if (item is FileViewModel)
                    File.Delete(FilePath);

                //тупое обновление страницы
                OnMoveBack(parameter);
                OnMoveForward(parameter);
                OpenDirectory();
            }
            else { throw new Exception(); }
        }        

        private bool OnCanDelete(object obj) => _history.CanDelete;
        #endregion

        #region Delete
        private void Rename(object parameter)
        {
            if (parameter is FileEntityViewModel item)
            {
                FilePath = item.FullName;                
                //if (item is DirectoryViewModel)

                //else if (item is FileViewModel)


                //тупое обновление страницы
                OnMoveBack(parameter);
                OnMoveForward(parameter);
                OpenDirectory();
            }
            else { throw new Exception(); }
        }        
        #endregion

        #region AddToQuickAccess
        private void AddToQuickAccess(object parameter) //!!Разобраться с сохранением коллекции
        {
            //XmlSerializer xsr = new XmlSerializer(typeof(ObservableCollection<FileEntityViewModel>));
            //StreamReader sr = new StreamReader(@"C:\Users\Anna\Documents\GitHub\FileExplorer\ViewModels\QuickAccessElements.xml");
            //quickAccessItems = xsr.Deserialize(sr) as ObservableCollection<FileEntityViewModel>;

            if (parameter is FileEntityViewModel item)
            {
                //bool append = true;
                //XmlSerializer xsw = new XmlSerializer(typeof(ObservableCollection<FileEntityViewModel>));
                //StreamWriter sw = new StreamWriter(@"C:\Users\Anna\Documents\GitHub\FileExplorer\ViewModels\QuickAccessElements.xml");
                if (!QuickAccessItems.Contains(item))
                {
                    QuickAccessItems.Add(item);
                    //xsw.Serialize(sw, quickAccessItems);
                }      
            }
            else { }
        }
        #endregion

        #region Tree
        private async Task OpenTree() 
        {
            Items = new ObservableCollection<FileEntityViewModel>();

            foreach (var logicalDrive in Directory.GetLogicalDrives())
            {                
                FileEntityViewModel root = new FileEntityViewModel(logicalDrive);
                root.FullName = Path.GetFullPath(logicalDrive);
                await Task.Run(() => root.Subfolders = GetSubfolders(logicalDrive));                    
                Items.Add(root);                
            }
        }
        
        private static ObservableCollection<FileEntityViewModel> GetSubfolders(string strPath)
        {
            ObservableCollection<FileEntityViewModel> subfolders = new();
            try
            {
                if (Directory.Exists(strPath))
                {
                    foreach (var dir in Directory.GetDirectories(strPath))
                    {
                        FileEntityViewModel thisnode = new FileEntityViewModel(dir);
                        if (((File.GetAttributes(dir) & (FileAttributes.System | FileAttributes.Hidden))
                            != (FileAttributes.System | FileAttributes.Hidden)) && Directory.Exists(dir))
                        {
                            thisnode.Name = Path.GetFileName(dir);
                            thisnode.FullName = Path.GetFullPath(dir);
                            subfolders.Add(thisnode);
                            //try { thisnode.Subfolders = GetSubfolders(dir); }
                            //catch (UnauthorizedAccessException) { }
                            //catch (FileNotFoundException) { }
                            //catch (DirectoryNotFoundException) { }
                        }
                    }


                    foreach (var file in Directory.GetFiles(strPath))
                    {
                        FileEntityViewModel thisnode = new FileEntityViewModel(file);
                        if ((File.GetAttributes(file) & (FileAttributes.System | FileAttributes.Hidden))
                            != (FileAttributes.System | FileAttributes.Hidden))
                        {
                            thisnode.Name = Path.GetFileName(file);
                            subfolders.Add(thisnode);
                        }
                    }
                }                
            }
            catch (UnauthorizedAccessException) { }
            //catch (FileNotFoundException) { }
            //catch (DirectoryNotFoundException) { }

            return subfolders;
        }
        #endregion

        #region Buttons
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
        #endregion
    }
}