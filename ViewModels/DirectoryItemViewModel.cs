using Avalonia.Controls;
using Avalonia.Controls.Generators;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileExplorer.ViewModels
{
    public partial class DirectoryItemViewModel : ListBox, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        //private BackgroundWorker _backgroundWorker;

        private readonly IDirectoryHistory _history;

        private readonly ISynchronizationHelper _synchronizationHelper;

        private const string QuickAccessFolderName = "quickAccessFolders.json";
        private const string QuickAccessFileName = "quickAccessFiles.json";
        FileInfo quickAccessFolderInfo = new FileInfo(QuickAccessFolderName);
        FileInfo quickAccessFileInfo = new FileInfo(QuickAccessFileName);

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
            private set
            {
                quickAccessItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QuickAccessItems)));
            }
        }

        private ObservableCollection<DirectoryViewModel> quickAccessDirectoryItems = new();
        public ObservableCollection<DirectoryViewModel> QuickAccessDirectoryItems
        {
            get => quickAccessDirectoryItems;
            private set { quickAccessDirectoryItems = value; }
        }

        private ObservableCollection<FileViewModel> quickAccessFileItems = new();
        public ObservableCollection<FileViewModel> QuickAccessFileItems
        {
            get => quickAccessFileItems;
            private set { quickAccessFileItems = value; }
        }

        private ObservableCollection<FileEntityViewModel> informationItems = new();
        public ObservableCollection<FileEntityViewModel> InformationItems
        {
            get => informationItems;
            private set
            {
                informationItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InformationItems)));
            }
        }

        #region Constructor
        public DirectoryItemViewModel(ISynchronizationHelper synchronizationHelper)
        {
            _history = new DirectoryHistory("Мой компьютер", "Мой компьютер");
            _synchronizationHelper = synchronizationHelper;

            Name = _history.Current.DirectoryPathName;
            FilePath = _history.Current.DirectoryPath;           

            OpenCommand = new DelegateCommand(Open);
            AddToQuickAccessCommand = new DelegateCommand(AddToQuickAccess);
            RemoveFromQuickAccessCommand = new DelegateCommand(RemoveFromQuickAccess);
            AddToInformationCommand = new DelegateCommand(AddToInformation);
            DeleteCommand = new DelegateCommand(Delete, OnCanDelete);
            RenameCommand = new DelegateCommand(Rename);
            //ReplaceCommand = new DelegateCommand(Replace, OnCanReplace);


            MoveBackCommand = new DelegateCommand(OnMoveBack, OnCanMoveBack);
            MoveForwardCommand = new DelegateCommand(OnMoveForward, OnCanMoveForward);
            MoveForwardCommand = new DelegateCommand(OnMoveUp, OnCanMoveUp);

            OpenDirectory();
            OpenTree();

            _history.HistoryChanged += History_HistoryChanged;

            QuickAccessItems = new ObservableCollection<FileEntityViewModel>();            
            ReadQuickAccessItem();

            //OpenBranchCommand = new DelegateCommand(OpenBranch);
            //KeyNavigationCommand = new DelegateCommand(KeyNavigation);
            //_history.KeyPress += KeyPressed;           
        }
        #endregion

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
        public DelegateCommand RemoveFromQuickAccessCommand { get; }
        public DelegateCommand AddToInformationCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand RenameCommand { get; }
        //public DelegateCommand ReplaceCommand { get; }


        public DelegateCommand MoveBackCommand { get; }
        public DelegateCommand MoveForwardCommand { get; }
        public DelegateCommand MoveUpCommand { get; } //разобраться с этой командой

        #region OpenDirectory
        private void Open(object parameter)
        {
            if (parameter is DirectoryViewModel directoryViewModel)
            {
                FilePath = directoryViewModel.FullName;
                Name = "Мой компьютер - " + directoryViewModel.Name;

                _history.Add(FilePath, Name);

                OpenDirectory();
            }
            else if (parameter is FileViewModel fileViewModel)
            {
                new Process
                {
                    StartInfo = new ProcessStartInfo(fileViewModel.FullName)
                    { 
                        UseShellExecute = true
                    }
                }.Start();
            }
            //else if (parameter == null)
            //{
            //    throw new ArgumentNullException(nameof(parameter));
            //}
        }
        
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
            try
            {
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    DirectoriesAndFiles.Add(new DirectoryViewModel(directory));
                }

                foreach (var fileInfo in directoryInfo.GetFiles())
                {
                    DirectoriesAndFiles.Add(new FileViewModel(fileInfo));
                }
            }
            catch (UnauthorizedAccessException) { }
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

        #region Replace
        private void Replace(object parameter)
        {
            if (parameter is FileEntityViewModel item)
            {
                string oldPath = item.FullName;
                if (item is DirectoryViewModel)
                {
                    string newPath = @"C:\Users\" + Path.GetFileName(oldPath);
                    //try
                    //{
                    var directoryInfo = new DirectoryInfo(oldPath);
                    directoryInfo.Attributes = FileAttributes.Normal;
                        DirectoriesAndFiles.Remove(item);
                        Directory.Move(oldPath, newPath);
                    //}
                    //catch (Exception) { }
                }                    
                //else if (item is FileViewModel)
                //{ 
                //    FileInfo fileInfo = new FileInfo(FilePath);
                //    fileInfo.MoveTo(@"D:\");
                //}                   
            }
            else { throw new Exception(); }
        }

        private bool OnCanReplace(object obj) => _history.CanReplace;
        #endregion

        #region Rename
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

        #region AddToQuickAccess & Delete

        JsonSerializerOptions options = new JsonSerializerOptions()
        { 
            AllowTrailingCommas = true,
            WriteIndented = true,
            IncludeFields = true,
        };

        private void AddToQuickAccess(object parameter)
        {      
            if (parameter is DirectoryViewModel fol_item)
            {
                foreach (var item in QuickAccessItems)
                {
                    if (item.FullName == fol_item.FullName) { return; }
                }
                QuickAccessItems.Add(fol_item);
                QuickAccessDirectoryItems.Add(fol_item);
                string fol_itemJson = JsonSerializer.Serialize<ObservableCollection<DirectoryViewModel>>(QuickAccessDirectoryItems, options);
                File.WriteAllText(QuickAccessFolderName, fol_itemJson);
            }
            else if (parameter is FileViewModel file_item)
            {
                foreach (var item in QuickAccessItems)
                {
                    if (item.FullName == file_item.FullName) { return; }
                }
                QuickAccessItems.Add(file_item);
                QuickAccessFileItems.Add(file_item);
                string file_itemJson = JsonSerializer.Serialize<ObservableCollection<FileViewModel>>(QuickAccessFileItems, options);
                File.WriteAllText(QuickAccessFileName, file_itemJson);
            }
        }

        private void ReadQuickAccessItem()
        {
            if (!quickAccessFolderInfo.Exists) { quickAccessFolderInfo.Create(); }
            if (!quickAccessFolderInfo.Exists) { quickAccessFolderInfo.Create(); }

            if (quickAccessFolderInfo.Length > 2)
            {
                string fol_dataJson = File.ReadAllText(QuickAccessFolderName);
                ObservableCollection<DirectoryViewModel>? fol_item = JsonSerializer.Deserialize<ObservableCollection<DirectoryViewModel>>(fol_dataJson);
                foreach (var item in fol_item)
                {
                    if (!QuickAccessItems.Contains(item) && Directory.Exists(item.FullName))
                    {
                        QuickAccessItems.Add(item);
                        QuickAccessDirectoryItems.Add(item);
                    }
                }                
            }
            if (quickAccessFileInfo.Length > 2)
            {
                string file_dataJson = File.ReadAllText(QuickAccessFileName);
                ObservableCollection<FileViewModel>? file_item = JsonSerializer.Deserialize<ObservableCollection<FileViewModel>>(file_dataJson);
                foreach (var item in file_item)
                {
                    if (!QuickAccessItems.Contains(item) && File.Exists(item.FullName))
                    {
                        QuickAccessItems.Add(item);
                        QuickAccessFileItems.Add(item);
                    }
                }
            }
        }

        private void RemoveFromQuickAccess(object parameter)
        {            
            if (parameter is DirectoryViewModel fol_item)
            {
                foreach (var item in QuickAccessItems)
                {
                    if (item.FullName == fol_item.FullName) 
                    {
                        QuickAccessItems.Remove(fol_item);
                        QuickAccessDirectoryItems.Remove(fol_item);

                        string fol_itemJson = JsonSerializer.Serialize<ObservableCollection<DirectoryViewModel>>(QuickAccessDirectoryItems, options);
                        quickAccessFolderInfo.Delete();
                        File.WriteAllText(QuickAccessFolderName, fol_itemJson);

                        return;
                    }
                } 
            }
            else if (parameter is FileViewModel file_item)
            {
                foreach (var item in QuickAccessItems)
                {
                    if (item.FullName == file_item.FullName)
                    {
                        QuickAccessItems.Remove(file_item);
                        QuickAccessFileItems.Remove(file_item);

                        string file_itemJson = JsonSerializer.Serialize<ObservableCollection<FileViewModel>>(QuickAccessFileItems, options);
                        quickAccessFileInfo.Delete();
                        File.WriteAllText(QuickAccessFileName, file_itemJson);

                        return;
                    }
                }                
            }
            else { throw new Exception(); }
        }
        #endregion

        #region InformationPanel
        private void AddToInformation(object parameter)
        {
            if (InformationItems.Count != 0)
            {
                InformationItems.Clear();
            }

            if (parameter is DirectoryViewModel fol_item)
            {
                try
                {
                    fol_item.NumberOfItems = Directory.GetDirectories(fol_item.FullName).Length +
                        Directory.GetFiles(fol_item.FullName).Length;
                }
                catch (Exception) { }
                InformationItems.Add(fol_item);
            }
            else if (parameter is FileViewModel file_item)
            {
                InformationItems.Add(file_item);
            }
            //else throw new Exception();
        }
        #endregion

        #region Tree
        public interface ISynchronizationHelper
        {
            Task InvokeAsync(Action action);
        }
        //private async Task OpenTree()
        private void OpenTree()
        {
            Items = new ObservableCollection<FileEntityViewModel>();

            foreach (var logicalDrive in Directory.GetLogicalDrives())
            {                
                FileEntityViewModel root = new FileEntityViewModel(logicalDrive);
                root.FullName = Path.GetFullPath(logicalDrive);
                //await Task.Run(() =>
                //{
                //    _synchronizationHelper.InvokeAsync(() =>
                //    {
                //        root.Subfolders = GetSubfolders(logicalDrive);
                //    });
                //});
                //await Task.Run(() => root.Subfolders = GetSubfolders(logicalDrive));
                root.Subfolders = GetSubfolders(logicalDrive);
                Items.Add(root);                
            }
        }
        
        private ObservableCollection<FileEntityViewModel> GetSubfolders(string strPath)
        {
            ObservableCollection<FileEntityViewModel> subfolders = new();
            try
            {
                if (Directory.Exists(strPath))
                {
                    foreach (var dir in Directory.EnumerateDirectories(strPath))
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