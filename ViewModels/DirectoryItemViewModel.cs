using Avalonia.Controls;
using Avalonia.Controls.Generators;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using static System.Net.WebRequestMethods;
using System.Reflection;
using System.Collections.Generic;
using FileExplorer.Views;
using DynamicData.Experimental;
using System.Net.Http.Headers;
using Avalonia.Controls.Shapes;
using DynamicData;
using System.Reflection.Metadata;
using System.Data.Common;
using Microsoft.VisualBasic;

namespace FileExplorer.ViewModels
{
    public partial class DirectoryItemViewModel : ListBox, INotifyPropertyChanged
    {
        //private BackgroundWorker _backgroundWorker;

        private readonly IDirectoryHistory _history;

        private readonly ISynchronizationHelper _synchronizationHelper;

        private const string QuickAccessFolderName = "quickAccessFolders.json";
        private const string QuickAccessFileName = "quickAccessFiles.json";
        FileInfo quickAccessFolderInfo = new FileInfo(QuickAccessFolderName);
        FileInfo quickAccessFileInfo = new FileInfo(QuickAccessFileName);

        #region EventProperties
        public event PropertyChangedEventHandler? PropertyChanged;

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

        private bool directoryWithLogicalDrives;
        public bool DirectoryWithLogicalDrives
        {
            get => directoryWithLogicalDrives;
            set
            {
                directoryWithLogicalDrives = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirectoryWithLogicalDrives)));
            }
        }
        #endregion

        #region BufferProperties
        private string pathBuffer;
        public string PathBuffer
        {
            get => pathBuffer;
            set => pathBuffer = value;
        }

        private string nameBuffer;
        public string NameBuffer
        {
            get => nameBuffer;
            set => nameBuffer = value; 
        }

        private int entityBuffer;
        public int EntityBuffer
        {
            get => entityBuffer;
            set => entityBuffer = value;
        }
        #endregion

        #region Collections
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

        private List<ObservableCollection<FileEntityViewModel>> Collections = new();
        #endregion

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
            CopyCommand = new DelegateCommand(Copy);
            CutCommand = new DelegateCommand(Cut);
            PasteCommand = new DelegateCommand(Paste);
            DeleteCommand = new DelegateCommand(Delete, OnCanDelete);
            RenameCommand = new DelegateCommand(Rename);
            CreateNewFolderCommand = new DelegateCommand(CreateNewFolder);

            AddToInformationCommand = new DelegateCommand(AddToInformation);
            SortByNameCommand = new DelegateCommand(SortByName);
            SortByDateOfChangeCommand = new DelegateCommand(SortByDateOfChange);
            SortByTypeCommand = new DelegateCommand(SortByType);
            SortBySizeCommand = new DelegateCommand(SortBySize);

            MoveBackCommand = new DelegateCommand(OnMoveBack, OnCanMoveBack);
            MoveForwardCommand = new DelegateCommand(OnMoveForward, OnCanMoveForward);
            MoveForwardCommand = new DelegateCommand(OnMoveUp, OnCanMoveUp);

            OpenDirectory();
            OpenTree();

            _history.HistoryChanged += History_HistoryChanged;

            QuickAccessItems = new ObservableCollection<FileEntityViewModel>();            
            ReadQuickAccessItem();

            Collections.Add(QuickAccessItems);
            Collections.Add(DirectoriesAndFiles);
            Collections.Add(Items);

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
        public DelegateCommand CopyCommand { get; }
        public DelegateCommand CutCommand { get; }
        public DelegateCommand PasteCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand RenameCommand { get; }
        public DelegateCommand CreateNewFolderCommand { get; }

        public DelegateCommand AddToInformationCommand { get; }
        public DelegateCommand SortByNameCommand { get; }
        public DelegateCommand SortByDateOfChangeCommand { get; }
        public DelegateCommand SortByTypeCommand { get; }
        public DelegateCommand SortBySizeCommand { get; }

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
                DirectoryWithLogicalDrives = true;
                foreach (var logicalDrive in Directory.GetLogicalDrives())
                {
                    DirectoriesAndFiles.Add(new DirectoryViewModel(logicalDrive));
                }
                return;
            }            

            DirectoryWithLogicalDrives = false;
            var directoryInfo = new DirectoryInfo(FilePath);

            FileSystemWatcher watcher = new FileSystemWatcher(FilePath);
            watcher.Path = FilePath;
            watcher.NotifyFilter = NotifyFilters.Attributes
                | NotifyFilters.CreationTime
                | NotifyFilters.DirectoryName
                | NotifyFilters.FileName
                | NotifyFilters.LastWrite
                | NotifyFilters.LastAccess
                | NotifyFilters.Size;

            watcher.Filter = "*.*";
            watcher.IncludeSubdirectories = true;

            watcher.Created += OnCreated;
            watcher.Changed += OnChanged;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;

            watcher.EnableRaisingEvents = true;
            watcher.InternalBufferSize = 4096;

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

        #region WatcherEvents        
        private async void OnCreated(object sender, FileSystemEventArgs e)
        {
            await Task.Run(() =>
            {
                _synchronizationHelper.InvokeAsync(() =>
                {
                    AddItemFromSystem(e);
                });
            });
        }
        private async Task AddItemFromSystem(FileSystemEventArgs e)
        {
            FileAttributes attr = System.IO.File.GetAttributes(e.FullPath);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                foreach (var item in DirectoriesAndFiles)
                {
                    if (item.FullName == e.FullPath) { return; }
                }
                DirectoryViewModel newDir = new DirectoryViewModel(e.Name);
                DirectoriesAndFiles.Add(newDir);
                newDir.FullName = e.FullPath;
            }
            else
            {
                if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                {
                    foreach (var item in DirectoriesAndFiles)
                    {
                        if (item.FullName == e.FullPath) { return; }
                    }
                    FileViewModel newFile = new FileViewModel(e.Name);
                    DirectoriesAndFiles.Add(newFile);
                    newFile.FullName = e.FullPath;
                }
            }
        }

        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            await Task.Run(() =>
            {
                _synchronizationHelper.InvokeAsync(() =>
                {
                    ChangedItemFromSystem(e);
                });
            });
        }
        private async Task ChangedItemFromSystem(FileSystemEventArgs e)
        {
            foreach (var collection in Collections)
            {
                foreach (var item in collection)
                {
                    if (item.FullName == e.FullPath)
                    {
                        collection.Remove(item);
                        if (item is DirectoryViewModel)
                        {
                            DirectoryInfo directoryInfo = new DirectoryInfo(e.FullPath);
                            collection.Add(new DirectoryViewModel(directoryInfo));
                        }
                        if (item is FileViewModel)
                        {
                            FileInfo fileInfo = new FileInfo(e.FullPath);
                            collection.Add(new FileViewModel(fileInfo));
                        }
                    }
                }
            }
            foreach (var item in QuickAccessDirectoryItems)
            {
                if (item.FullName == e.FullPath)
                {
                    QuickAccessDirectoryItems.Remove(item);
                    DirectoryInfo directoryInfo = new DirectoryInfo(e.FullPath);
                    QuickAccessDirectoryItems.Add(new DirectoryViewModel(directoryInfo));
                }
            }
            foreach (var item in QuickAccessFileItems)
            {
                if (item.FullName == e.FullPath)
                {
                    QuickAccessFileItems.Remove(item);
                    FileInfo fileInfo = new FileInfo(e.FullPath);
                    QuickAccessFileItems.Add(new FileViewModel(fileInfo));
                }
            }            
        }

        private async void OnDeleted(object sender, FileSystemEventArgs e)
        {
            await Task.Run(() =>
            {
                _synchronizationHelper.InvokeAsync(() =>
                {
                    DeletedItemFromSystem(e);
                });
            });
        }
        private async Task DeletedItemFromSystem(FileSystemEventArgs e)
        {
            foreach (var collection in Collections)
            {
                foreach(var item in collection)
                {
                    if (item.FullName == e.FullPath)
                    {
                        collection.Remove(item);
                    }
                }
            }
            foreach (var item in QuickAccessDirectoryItems)
            {
                if (item.FullName == e.FullPath)
                {
                    QuickAccessDirectoryItems.Remove(item);
                }
            }
            foreach (var item in QuickAccessFileItems)
            {
                if (item.FullName == e.FullPath)
                {
                    QuickAccessFileItems.Remove(item);
                }
            }

            //тупое обновление страницы
            OnMoveBack(e.FullPath);
            OnMoveForward(e.FullPath);
            OpenDirectory();
        }

        private async void OnRenamed(object sender, RenamedEventArgs e)
        {           
            await Task.Run(() =>
            {
                _synchronizationHelper.InvokeAsync(() =>
                {
                    RenamedItemFromSystem(e);
                });
            });
        }
        private async Task RenamedItemFromSystem(RenamedEventArgs e)
        {
            foreach (var collection in Collections)
            {
                foreach (var item in collection)
                {
                    if (item.FullName == e.OldFullPath)
                    {
                        item.FullName = e.FullPath;
                        item.Name = e.Name;
                    }
                }
            }
            foreach (var item in QuickAccessDirectoryItems)
            {
                if (item.FullName == e.OldFullPath)
                {
                    item.FullName = e.FullPath;
                    item.Name = e.Name;
                }
            }
            foreach (var item in QuickAccessFileItems)
            {
                if (item.FullName == e.OldFullPath)
                {
                    item.FullName = e.FullPath;
                    item.Name = e.Name;
                }
            }

            //тупое обновление страницы
            OnMoveBack(e.FullPath);
            OnMoveForward(e.FullPath);
            OpenDirectory();
        }
        #endregion

        #region Copy&CutAndPaste
        private List<string> ItemBuffer = new();
        //private void Copy(object parameter)
        //{
        //    if (parameter is FileEntityViewModel item)
        //    {
        //        PathBuffer = item.FullName;
        //        NameBuffer = item.Name;

        //        if (item is DirectoryViewModel) 
        //        { 
        //            EntityBuffer = 2;
        //        }
        //        else if (item is FileViewModel) 
        //        { 
        //            EntityBuffer = 1;
        //        }
        //        else { EntityBuffer = 0; }
        //    }
        //    else { throw new Exception(); }
        //}
        private void Copy(object parameter)
        {
            if (parameter is FileEntityViewModel item)
            {
                ItemBuffer.Clear();
                AddToItemBuffer(item.FullName);
            }
            else { throw new Exception(); }
        }
        private void AddToItemBuffer(string path)
        {          
            ItemBuffer.Add(path);

            FileAttributes attributes = System.IO.File.GetAttributes(path);
            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory) //директория
            {
                if (Directory.EnumerateDirectories(path).Count() != 0)
                {
                    foreach (string dir in Directory.EnumerateDirectories(path))
                    {
                        AddToItemBuffer(dir);
                    }
                }
                if (Directory.EnumerateFiles(path).Count() != 0)
                {
                    foreach (string file in Directory.EnumerateFiles(path))
                    {
                        ItemBuffer.Add(file);
                    }
                }
            }
        }
        private void Cut(object parameter)
        {
            if (parameter is FileEntityViewModel item)
            {
                Copy(parameter);
                DirectoriesAndFiles.Remove(item); 
                Directory.Delete(item.FullName, true);  
            }
            else { throw new Exception(); }
        }

        private string GetNameOfCopiedItem(DirectoryInfo directoryInfo, string name, int extention = 0)
        {            
            var dirs = directoryInfo.EnumerateDirectories();
            var files = directoryInfo.EnumerateFiles();

            while (true)
            {
                bool copy = false;
                foreach (var d in dirs)
                {
                    if (name == d.Name)
                    {
                        name = name + " —" + " копия";
                        copy = true;
                    }
                }
                foreach (var f in files)
                {
                    if (name == f.Name[..^extention]) 
                    {
                        name = name + " —" + " копия";
                        copy = true;
                    }
                }
                if (copy == false) break;
            }
            return name;
        }

        //private async Task PasteSubitems(string oldPath, string newPath)
        //{
        //    var subdirectories = Directory.GetDirectories(oldPath);
        //    if (subdirectories.Count() != 0)
        //    {
        //        foreach (string subdirectory in subdirectories)
        //        {
        //            try
        //            {
        //                string nameOfSubdirectory = System.IO.Path.GetFileName(subdirectory);
        //                Directory.CreateDirectory(subdirectory.Replace(oldPath + @"\" + nameOfSubdirectory, newPath + @"\" + nameOfSubdirectory));
        //                await Task.Run(() => 
        //                {
        //                    _synchronizationHelper.InvokeAsync(() =>
        //                    {
        //                        PasteSubitems(oldPath + @"\" + System.IO.Path.GetFileName(subdirectory),
        //                              newPath + @"\" + System.IO.Path.GetFileName(subdirectory));
        //                    });
        //                });                        
        //            }
        //            catch (Exception e) { }
        //        }
        //    }

        //    var subfiles = Directory.GetFiles(oldPath);
        //    if (subfiles.Count() != 0)
        //    {
        //        foreach (string subfile in subfiles) 
        //        {
        //            try
        //            {
        //                string nameOfSubfile = System.IO.Path.GetFileName(subfile);
        //                System.IO.File.Copy(oldPath + @"\" + nameOfSubfile, newPath + @"\" + nameOfSubfile, false);
        //            }
        //            catch (Exception e) { }
        //        }
        //    }            
        //}

        //private async void Paste(object parameter) // добавить try-catch??
        //{
        //    if (parameter is string directory && directory != "Мой компьютер")
        //    {
        //        var directoryInfo = new DirectoryInfo(directory);

        //        if (EntityBuffer == 2) //не копируется содержимое папки
        //        {                 
        //            var pastedFolder = new DirectoryViewModel(PathBuffer);                   

        //            pastedFolder.Name = GetNameOfCopiedItem(directoryInfo, NameBuffer);
        //            pastedFolder.FullName = directoryInfo.FullName + @"\" + pastedFolder.Name;
        //            pastedFolder.DateOfChange = directoryInfo.LastWriteTime.ToShortDateString() + " " + directoryInfo.LastWriteTime.ToShortTimeString();
        //            DirectoriesAndFiles.Add(pastedFolder);

        //            Directory.CreateDirectory(pastedFolder.FullName);

        //            await Task.Run(() => //все равно виснет
        //            {
        //                _synchronizationHelper.InvokeAsync(() =>
        //                {
        //                    PasteSubitems(PathBuffer, pastedFolder.FullName);
        //                });
        //            });  
        //        }
        //        else if (EntityBuffer == 1)
        //        {
        //            var fileInfo = new FileInfo(directory);
        //            var pastedFile = new FileViewModel(PathBuffer);
        //            string extention = System.IO.Path.GetExtension(PathBuffer);
        //            NameBuffer = NameBuffer[..^extention.Length];

        //            pastedFile.Name = GetNameOfCopiedItem(directoryInfo, NameBuffer, extention.Length) + extention;
        //            pastedFile.FullName = fileInfo.FullName + @"\" + pastedFile.Name;
        //            pastedFile.DateOfCreation = fileInfo.CreationTime.ToShortDateString() + " " + fileInfo.CreationTime.ToShortTimeString();
        //            pastedFile.DateOfChange = fileInfo.LastWriteTime.ToShortDateString() + " " + fileInfo.LastWriteTime.ToShortTimeString();
        //            DirectoriesAndFiles.Add(pastedFile);

        //            System.IO.File.Copy(PathBuffer, pastedFile.FullName, false);                  
        //        }
        //        else throw new Exception();                               
        //    }
        //    else { throw new Exception(); }
        //}
        private async void Paste(object parameter) // добавить try-catch??
        {
            string mainDirectory = "";
            if (parameter is string directory && directory != "Мой компьютер")
            {
                foreach (string itemPath in ItemBuffer)
                {    
                    if (itemPath == ItemBuffer[0]) //корневая директория / одиночный файл
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(directory);

                        FileAttributes attributes = System.IO.File.GetAttributes(itemPath);
                        if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            var pastedFolder = new DirectoryViewModel(itemPath);

                            pastedFolder.Name = GetNameOfCopiedItem(directoryInfo, System.IO.Path.GetFileName(itemPath));
                            pastedFolder.FullName = directoryInfo.FullName + @"\" + pastedFolder.Name;
                            mainDirectory = pastedFolder.FullName;
                            pastedFolder.DateOfChange = directoryInfo.LastWriteTime.ToShortDateString() + " " 
                                + directoryInfo.LastWriteTime.ToShortTimeString();
                            DirectoriesAndFiles.Add(pastedFolder);
                            Directory.CreateDirectory(pastedFolder.FullName);
                        }                       
                        else
                        {
                            var fileInfo = new FileInfo(directory);
                            var pastedFile = new FileViewModel(itemPath);
                            string extention = System.IO.Path.GetExtension(itemPath);
                            string pureName = System.IO.Path.GetFileName(itemPath);
                            pureName = pureName.Substring(0, pureName.Length - extention.Length);

                            pastedFile.Name = GetNameOfCopiedItem(directoryInfo, pureName, extention.Length) + extention;
                            pastedFile.FullName = fileInfo.FullName + @"\" + pastedFile.Name;                            
                            pastedFile.DateOfCreation = fileInfo.CreationTime.ToShortDateString() + " " 
                                + fileInfo.CreationTime.ToShortTimeString();
                            pastedFile.DateOfChange = fileInfo.LastWriteTime.ToShortDateString() + " " 
                                + fileInfo.LastWriteTime.ToShortTimeString();
                            DirectoriesAndFiles.Add(pastedFile);

                            System.IO.File.Copy(itemPath, pastedFile.FullName, false);
                        }
                    }
                    else //поддиректории / файлы внутри ДОДЕЛАТЬ
                    {
                        string _itemPath = itemPath.Replace(ItemBuffer[0], mainDirectory);
                        //DirectoryInfo directoryInfo = new DirectoryInfo(itemPath);

                        FileAttributes attributes = System.IO.File.GetAttributes(itemPath);
                        if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        {                                                    
                            Directory.CreateDirectory(_itemPath);
                        }
                        else // ДОДЕЛАТЬ
                        {
                            FileInfo fileInfo = new FileInfo(directory);

                            string extention = System.IO.Path.GetExtension(_itemPath);
                            string name = System.IO.Path.GetFileName(_itemPath)[..^extention.Length];

                            //name = GetNameOfCopiedItem(directoryInfo, System.IO.Path.GetFileName(_itemPath), extention.Length) + extention;
                            string path = fileInfo.FullName + @"\" + name;                            

                            System.IO.File.Copy(_itemPath, path, false);
                        }
                    }
                }                          
            }
            else { throw new Exception(); }
        }
        #endregion

        #region Delete    

        private void Delete(object parameter)
        {
            if (parameter is FileEntityViewModel item)
            {                
                FilePath = item.FullName;
                DirectoryInfo di = new DirectoryInfo(FilePath);
                if (item is DirectoryViewModel)
                {                                   
                    foreach (FileInfo file in di.EnumerateFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.EnumerateDirectories())
                    {
                        dir.Delete(true);
                    }

                    Directory.Delete(FilePath);                    
                }                
                else if (item is FileViewModel)
                {
                    System.IO.File.Delete(FilePath);
                }

                DirectoriesAndFiles.Remove(item);                
            }
            else { throw new Exception(); }
        }        
        private void Delete(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }
            Directory.Delete(path);
        }        

        private bool OnCanDelete(object obj) => _history.CanDelete; //возможно, не нужно (опция скрыта, когда на гл. экране только лог. диски)    
        #endregion 

        #region Replace
        private void Replace(object parameter)
        {
            if (parameter is FileEntityViewModel item)
            {
                string oldPath = item.FullName;
                if (item is DirectoryViewModel)
                {
                    string newPath = @"C:\Users\" + System.IO.Path.GetFileName(oldPath);
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

        #region Add&Remove_QuickAccess 
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
                System.IO.File.WriteAllText(QuickAccessFolderName, fol_itemJson);
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
                System.IO.File.WriteAllText(QuickAccessFileName, file_itemJson);
            }
        }
        private void ReadQuickAccessItem()
        {
            if (!quickAccessFolderInfo.Exists) { quickAccessFolderInfo.Create(); }
            if (!quickAccessFolderInfo.Exists) { quickAccessFolderInfo.Create(); }

            if (quickAccessFolderInfo.Length > 2)
            {
                string fol_dataJson = System.IO.File.ReadAllText(QuickAccessFolderName);
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
                string file_dataJson = System.IO.File.ReadAllText(QuickAccessFileName);
                ObservableCollection<FileViewModel>? file_item = JsonSerializer.Deserialize<ObservableCollection<FileViewModel>>(file_dataJson);
                foreach (var item in file_item)
                {
                    if (!QuickAccessItems.Contains(item) && System.IO.File.Exists(item.FullName))
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
                        System.IO.File.WriteAllText(QuickAccessFolderName, fol_itemJson);

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
                        System.IO.File.WriteAllText(QuickAccessFileName, file_itemJson);

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

        #region Sorting
        private void AddSortedItems(IOrderedEnumerable<DirectoryInfo> directories, IOrderedEnumerable<FileInfo> files)
        {
            DirectoriesAndFiles.Clear();

            try
            {
                foreach (var directory in directories)
                {
                    DirectoriesAndFiles.Add(new DirectoryViewModel(directory));
                }

                foreach (var file in files)
                {
                    DirectoriesAndFiles.Add(new FileViewModel(file));
                }
            }
            catch (Exception e) { }
        }

        private void SortByName(object parameter) 
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(parameter.ToString());

            if (MainWindow.asc == true)
            {
                var dirs = directoryInfo.EnumerateDirectories().OrderBy(d => d.Name);
                var files = directoryInfo.EnumerateFiles().OrderBy(d => d.Name);
                AddSortedItems(dirs, files);
            }
            if (MainWindow.desc == true) 
            {
                var dirs = directoryInfo.EnumerateDirectories().OrderByDescending(d => d.Name);
                var files = directoryInfo.EnumerateFiles().OrderByDescending(d => d.Name);
                AddSortedItems(dirs, files);
            }  
        }

        private void SortByDateOfChange(object parameter) 
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(parameter.ToString());

            if (MainWindow.asc == true)
            {
                var dirs = directoryInfo.EnumerateDirectories().OrderBy(d => d.LastWriteTime);
                var files = directoryInfo.EnumerateFiles().OrderBy(d => d.LastWriteTime);
                AddSortedItems(dirs, files);
            }
            if (MainWindow.desc == true)
            {
                var dirs = directoryInfo.EnumerateDirectories().OrderByDescending(d => d.LastWriteTime);
                var files = directoryInfo.EnumerateFiles().OrderByDescending(d => d.LastWriteTime);
                AddSortedItems(dirs, files);
            }
        }

        private void SortByType(object parameter)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(parameter.ToString());

            if (MainWindow.asc == true)
            {
                var dirs = directoryInfo.EnumerateDirectories().OrderBy(d => d.Extension);
                var files = directoryInfo.EnumerateFiles().OrderBy(d => d.Extension);
                AddSortedItems(dirs, files);
            }
            if (MainWindow.desc == true)
            {
                var dirs = directoryInfo.EnumerateDirectories().OrderByDescending(d => d.Extension);
                var files = directoryInfo.EnumerateFiles().OrderByDescending(d => d.Extension);
                AddSortedItems(dirs, files);
            }
        }

        //public static long GetDirectorySize(string path)
        //{
        //    DirectoryInfo dir = new DirectoryInfo(path);
        //    try
        //    {
        //        return dir.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(f => f.Length);
        //    }
        //    catch (Exception e) { return 0; }
        //}
        
        private void SortBySize(object parameter) //доделать
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(parameter.ToString());
            
            List<FileInfo> fileInfos = directoryInfo.GetFiles().ToList();

            var dirs = directoryInfo.EnumerateDirectories().OrderBy(d => d.Name); //Не удается корректно подсчитать размер директории            
            if (MainWindow.asc == true)
            {
                var files = fileInfos.Where(f => f.FullName != null).OrderBy(f => f.Length);
                AddSortedItems(dirs, files);
            }
            if (MainWindow.desc == true)
            {
                var files = fileInfos.Where(f => f.FullName != null).OrderByDescending(f => f.Length);
                AddSortedItems(dirs, files);
            }      
        }
        #endregion

        #region NewFolder
        private string GetNameOfNewFolder(DirectoryInfo directoryInfo, string name)
        {
            var dirs = directoryInfo.EnumerateDirectories();
            var files = directoryInfo.EnumerateFiles();
            int counter = 1;

            while (true)
            {
                int temp = counter;
                foreach (var d in dirs)
                {
                    if (name == d.Name)
                    {
                        name = name + '(' + counter + ')';
                        counter++;
                    }
                }
                foreach (var f in files)
                {
                    if (name == f.Name)
                    {
                        name = name + '(' + counter + ')';
                        counter++;
                    }
                }
                if (temp - counter == 0) break;
            }
            if (counter != 1) counter = 1;
            return name;
        }
        private void CreateNewFolder(object parameter)
        {
            if (parameter is string directory && directory != "Мой компьютер")
            {                
                var directoryInfo = new DirectoryInfo(directory);
                System.IO.File.SetAttributes(directory, FileAttributes.Normal);                

                var newFolder = new DirectoryViewModel(directory);
                DirectoriesAndFiles.Add(newFolder);

                newFolder.Name = GetNameOfNewFolder(directoryInfo, "Новая папка");
                newFolder.FullName = directoryInfo.FullName + @"\" + newFolder.Name;
                newFolder.IsSystemFolder = false;
                newFolder.Type = "Папка с файлами";
                
                Directory.CreateDirectory(newFolder.FullName);
            }
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
                root.FullName = System.IO.Path.GetFullPath(logicalDrive);
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
                        if (((System.IO.File.GetAttributes(dir) & (FileAttributes.System | FileAttributes.Hidden))
                            != (FileAttributes.System | FileAttributes.Hidden)) && Directory.Exists(dir))
                        {
                            thisnode.Name = System.IO.Path.GetFileName(dir);
                            thisnode.FullName = System.IO.Path.GetFullPath(dir);
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
                        if ((System.IO.File.GetAttributes(file) & (FileAttributes.System | FileAttributes.Hidden))
                            != (FileAttributes.System | FileAttributes.Hidden))
                        {
                            thisnode.Name = System.IO.Path.GetFileName(file);
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