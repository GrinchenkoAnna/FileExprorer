﻿using Avalonia.Controls;

using FileExplorer.Views;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using static FileExplorer.ViewModels.DirectoryItemViewModel;

namespace FileExplorer.ViewModels
{
    public partial class DirectoryItemViewModel : INotifyPropertyChanged
    {
        //private BackgroundWorker _backgroundWorker;

        private readonly IDirectoryHistory _history;
        public ISynchronizationHelper? _synchronizationHelper;

        public DelegateCommand OpenCommand { get; }
        public DelegateCommand AddToQuickAccessCommand { get; }
        public DelegateCommand RemoveFromQuickAccessCommand { get; }
        public DelegateCommand CopyCommand { get; }
        public DelegateCommand CutCommand { get; }
        public DelegateCommand PasteCommand { get; }
        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand RenameCommand { get; }
        public DelegateCommand CreateNewFolderCommand { get; }
        public DelegateCommand ShowItemPropertiesCommand { get; }
        public DelegateCommand AddToInformationCommand { get; }
        public DelegateCommand SortByNameCommand { get; }
        public DelegateCommand SortByDateOfChangeCommand { get; }
        public DelegateCommand SortByTypeCommand { get; }
        public DelegateCommand SortBySizeCommand { get; }
        public DelegateCommand RefreshSortCommand { get; }
        public DelegateCommand MoveBackCommand { get; }
        public DelegateCommand MoveForwardCommand { get; }
        public DelegateCommand MoveUpCommand { get; }
        public DelegateCommand SearchCommand { get; }

        private const string QuickAccessFolderName = "ViewModels\\quickAccessFolders.json";
        private const string QuickAccessFileName = "ViewModels\\quickAccessFiles.json";
        FileInfo quickAccessFolderInfo = new FileInfo(QuickAccessFolderName);
        FileInfo quickAccessFileInfo = new FileInfo(QuickAccessFileName);

        #region EventProperties
        public event PropertyChangedEventHandler? PropertyChanged;

        protected string filePath;
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

        private string currentSearchDirectory;
        public string CurrentSearchDirectory
        {
            get => currentSearchDirectory;
            set
            {
                currentSearchDirectory = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentSearchDirectory)));
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
        public List<string> ItemBuffer = new();
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

        private ObservableCollection<FileEntityViewModel> treeItems = new();
        public ObservableCollection<FileEntityViewModel> TreeItems
        {
            get => treeItems;
            set
            {
                treeItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TreeItems)));
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

        private ObservableCollection<FileEntityViewModel> propertiesOfItems = new();
        public ObservableCollection<FileEntityViewModel> PropertiesOfItems
        {
            get => propertiesOfItems;
            private set
            {
                propertiesOfItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PropertiesOfItems)));
            }
        }

        private ObservableCollection<FileEntityViewModel> itemsToSearch = new();
        public ObservableCollection<FileEntityViewModel> ItemsToSearch
        {
            get => itemsToSearch;
            private set
            {
                itemsToSearch = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemsToSearch)));
            }
        }
        protected List<ObservableCollection<FileEntityViewModel>> Collections = new();
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

            ShowItemPropertiesCommand = new DelegateCommand(ShowItemProperties);

            AddToInformationCommand = new DelegateCommand(AddToInformation);
            SortByNameCommand = new DelegateCommand(SortByName);
            SortByDateOfChangeCommand = new DelegateCommand(SortByDateOfChange);
            SortByTypeCommand = new DelegateCommand(SortByType);
            SortBySizeCommand = new DelegateCommand(SortBySize);
            RefreshSortCommand = new DelegateCommand(RefreshSort);

            MoveBackCommand = new DelegateCommand(OnMoveBack, OnCanMoveBack);
            MoveForwardCommand = new DelegateCommand(OnMoveForward, OnCanMoveForward);
            MoveUpCommand = new DelegateCommand(OnMoveUp, OnCanMoveUp);

            SearchCommand = new DelegateCommand(Search);

            OpenDirectory();
            OpenTree();

            _history.HistoryChanged += History_HistoryChanged;

            QuickAccessItems = new ObservableCollection<FileEntityViewModel>();
            QuickAccessDirectoryItems = new ObservableCollection<DirectoryViewModel>();
            QuickAccessFileItems = new ObservableCollection<FileViewModel>();
            ReadQuickAccessItem();

            Collections.Add(QuickAccessItems);
            Collections.Add(DirectoriesAndFiles);
            Collections.Add(TreeItems);
            Collections.Add(ItemsToSearch);                      
        }
        #endregion

        private void History_HistoryChanged(object? sender, EventArgs e)
        {
            MoveBackCommand?.RaiseCanExecuteChanged();
            MoveUpCommand?.RaiseCanExecuteChanged();
            MoveForwardCommand?.RaiseCanExecuteChanged();
        }  
        #region OpenDirectory
        public void Open(object parameter)
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
        public void OpenDirectory()
        {
            DirectoriesAndFiles.Clear();            
            if (Name == "Мой компьютер")
            {
                DirectoryWithLogicalDrives = true;
                foreach (var logicalDrive in Directory.GetLogicalDrives())
                {
                    DirectoriesAndFiles.Add(new DirectoryViewModel(logicalDrive));
                }
                CurrentSearchDirectory = "Для поиска зайдите в локальный диск, введите текст и нажмите кнопку 'Найти'";

                return;
            }
            else
            {
                DirectoryWithLogicalDrives = false;

                char[] charsToTrim = { '\\', ':' };
                CurrentSearchDirectory = "Поиск в " + Name.Substring(16).Trim(charsToTrim) + " ";

                var watcher = new Watcher(_synchronizationHelper, FilePath);
                DirectoryInfo directoryInfo = watcher.StartWatcher();

                try
                {
                    ItemsToSearch.Clear();
                    //await Task.Run(() =>
                    //{
                    //    _synchronizationHelper.InvokeAsync(() =>
                    //    {
                    //        GetSubItems(FilePath);
                    //    });
                    //});   
                    //GetSubItems(FilePath);               

                    foreach (var directory in directoryInfo.GetDirectories())
                    {
                        DirectoriesAndFiles.Add(new DirectoryViewModel(directory));
                        ItemsToSearch.Add(new DirectoryViewModel(directory));
                    }

                    foreach (var fileInfo in directoryInfo.GetFiles())
                    {
                        DirectoriesAndFiles.Add(new FileViewModel(fileInfo));
                        ItemsToSearch.Add(new FileViewModel(fileInfo));
                    }
                }
                catch (UnauthorizedAccessException) { }
            }

            //private async void GetSubItems(string path)
            //{
            //    try
            //    {
            //        foreach (var directory in Directory.EnumerateDirectories(path))
            //        {
            //            ItemsToSearch.Add(new DirectoryViewModel(directory));
            //            //GetSubItems(directory); виснет, если перебирать все папки
            //        }
            //        foreach (var file in Directory.EnumerateFiles(path))
            //        {
            //            ItemsToSearch.Add(new FileViewModel(file));
            //        }
            //    }
            //    catch (UnauthorizedAccessException) { }
            //}           
        }
        #endregion

        #region Copy&CutAndPaste
        bool cutItem = false;

        public void Copy(object parameter)
        {
            if (parameter is FileEntityViewModel item)
            {
                if (ItemBuffer.Count != 0)
                {
                    ItemBuffer.Clear();
                }
                AddToItemBuffer(item.FullName);
            }
            else { throw new Exception(); }
        }
        public void AddToItemBuffer(string path)
        {
            ItemBuffer.Add(path);

            FileAttributes attributes = System.IO.File.GetAttributes(path);
            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory) //директория
            {
                if (Directory.EnumerateDirectories(path).Any())
                {
                    foreach (string dir in Directory.EnumerateDirectories(path))
                    {
                        AddToItemBuffer(dir);
                    }
                }
                if (Directory.EnumerateFiles(path).Any())
                {
                    foreach (string file in Directory.EnumerateFiles(path))
                    {
                        ItemBuffer.Add(file);
                    }
                }
            }
        }
        public void Cut(object parameter)
        {
            if (parameter is FileEntityViewModel item)
            {
                cutItem = true;
                Copy(parameter);
                //DirectoriesAndFiles.Remove(item); //сделать полупрозрачным?            
            }
            else { throw new Exception(); }
        }

        public string GetNameOfCopiedItem(DirectoryInfo directoryInfo, string name, int extention = 0)
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

        public async Task PasteSubitems(string oldPath, string beginningOfNewPath)
        {
            string newPath = oldPath.Replace(ItemBuffer[0], beginningOfNewPath);

            FileAttributes attributes = System.IO.File.GetAttributes(oldPath);
            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Directory.CreateDirectory(newPath);
            }
            else
            {
                System.IO.File.Copy(oldPath, newPath, false);
            }
        }

        public async void Paste(object parameter) // добавить try-catch??
        {
            string mainDirectory = ""; //новое имя корневой директории
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
                    else //поддиректории / файлы внутри
                    {
                        await Task.Run(() => //все равно виснет
                        {
                            _synchronizationHelper.InvokeAsync(() =>
                            {
                                PasteSubitems(itemPath, mainDirectory);
                            });
                        });
                    }

                    if (cutItem)
                    {
                        Delete(ItemBuffer[0]);
                        cutItem = false;
                    }
                }
            }
            else { throw new Exception(); }
        }
        #endregion

        #region Delete   
        public void Delete(object parameter)
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

                    Directory.Delete(FilePath, true);
                }
                else if (item is FileViewModel)
                {
                    System.IO.File.Delete(FilePath);
                }

                DirectoriesAndFiles.Remove(item);
            }
            else { throw new Exception(); }
        }
        public void Delete(string path)
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
            Directory.Delete(path, true);

            var itemToRemove = DirectoriesAndFiles.Single(i => i.FullName == path);
            DirectoriesAndFiles.Remove(itemToRemove);
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

        public void AddToQuickAccess(object parameter)
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

        public void AddDefaultFolders(string folder, string name)
        {
            bool copy = false;
            
            DirectoryViewModel default_folder = new DirectoryViewModel(folder);
            default_folder.Name = name;
            foreach (var item in QuickAccessItems)
            {
                if (item.FullName == folder) { copy = true; }
            }
            if (copy) { copy = false; }
            else
            {
                QuickAccessItems.Add(default_folder);
                QuickAccessDirectoryItems.Add(default_folder);
            }
        }
        private void ReadQuickAccessItem()
        {
            if (!quickAccessFolderInfo.Exists) { quickAccessFolderInfo.Create(); }
            if (!quickAccessFileInfo.Exists) { quickAccessFileInfo.Create(); }

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
            AddDefaultFolders(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Документы");
            AddDefaultFolders(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Изображения");
            AddDefaultFolders(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Рабочий стол");
            AddDefaultFolders(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads", "Загрузки");
        }
        public void RemoveFromQuickAccess(object parameter)
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
            //else { throw new Exception(); }
        }
        #endregion

        #region InformationPanel        
        public void AddToInformation(object parameter)
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
        public string theLastSort = "name";
        public void RefreshSort(object parameter)
        {
            if (parameter is string)
            {
                if (theLastSort == "name")
                {
                    SortByName(parameter);
                }
                else if (theLastSort == "dateOfChange")
                {
                    SortByDateOfChange(parameter);
                }
                else if (theLastSort == "type")
                {
                    SortByType(parameter);
                }
                else if (theLastSort == "size")
                {
                    SortBySize(parameter);
                }
                else { }
            }
            else { throw new Exception(); }
        }

        public void AddSortedItems(IOrderedEnumerable<DirectoryInfo> directories, IOrderedEnumerable<FileInfo> files)
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

        public void SortByName(object parameter)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(parameter.ToString()); ;
            
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
            theLastSort = "name";
        }

        public void SortByDateOfChange(object parameter)
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
            theLastSort = "dateOfChange";
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
            theLastSort = "type";
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
            theLastSort = "size";
        }
        #endregion

        #region NewFolder
        public string GetNameOfNewFolder(DirectoryInfo directoryInfo, string name)
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
        public void CreateNewFolder(object parameter)
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

        #region ShowItemProperties
        private void ShowItemProperties(object parameter)
        {
            if (parameter is FileEntityViewModel item)
            {
                PropertiesOfItems.Clear();
                PropertiesOfItems.Add(item);

                //MainWindow.ShowPropertiesWindow();
            }
            //else { throw new Exception(); }
        }
        #endregion

        #region Tree
        public interface ISynchronizationHelper
        {
            Task InvokeAsync(Action action);
        }
        //public async Task OpenTree()
        public void OpenTree()
        {
            TreeItems = new ObservableCollection<FileEntityViewModel>();

            foreach (var logicalDrive in Directory.GetLogicalDrives())
            {
                FileEntityViewModel root = new FileEntityViewModel(logicalDrive);
                root.FullName = System.IO.Path.GetFullPath(logicalDrive);
                TreeItems.Add(root);
                //await Task.Run(() =>
                //{
                //    _synchronizationHelper.InvokeAsync(() =>
                //    {
                //        root.Subfolders = GetSubfolders(logicalDrive);
                //    });
                //});
                //await Task.Run(() => root.Subfolders = GetSubfolders(logicalDrive));
                root.Subfolders = GetSubfolders(logicalDrive);                
            }
        }

        public ObservableCollection<FileEntityViewModel> GetSubfolders(string strPath)
        {
            ObservableCollection<FileEntityViewModel> subfolders = new();
            try
            {
                if (Directory.Exists(strPath))
                {
                    foreach (var dir in Directory.EnumerateDirectories(strPath))
                    {
                        FileEntityViewModel thisnode = new FileEntityViewModel(dir);

                        thisnode.Name = System.IO.Path.GetFileName(dir);
                        thisnode.FullName = System.IO.Path.GetFullPath(dir);
                        subfolders.Add(thisnode);
                        //try { thisnode.Subfolders = GetSubfolders(dir); }
                        //catch (UnauthorizedAccessException) { }
                    }
                    foreach (var file in Directory.GetFiles(strPath))
                    {
                        FileEntityViewModel thisnode = new FileEntityViewModel(file);

                        thisnode.Name = System.IO.Path.GetFileName(file);
                        thisnode.FullName = System.IO.Path.GetFullPath(file);
                        subfolders.Add(thisnode);
                    }
                }
            }
            catch (UnauthorizedAccessException) { }

            return subfolders;
        }
        #endregion

        #region Buttons
        public void OnMoveBack(object obj)
        {
            _history.MoveBack();

            var current = _history.Current;
            FilePath = current.DirectoryPath;
            Name = current.DirectoryPathName;

            OpenDirectory();
        }
        private bool OnCanMoveBack(object obj) => _history.CanMoveBack;

        public void OnMoveForward(object obj)
        {
            _history.MoveForward();

            var current = _history.Current;
            FilePath = current.DirectoryPath;
            Name = current.DirectoryPathName;

            OpenDirectory();
        }
        private bool OnCanMoveForward(object obj) => _history.CanMoveForward;

        public void OnMoveUp(object obj)
        {
            _history.MoveUp();

            var current = _history.Current;
            FilePath = current.DirectoryPath;
            Name = current.DirectoryPathName;

            OpenDirectory();
        }
        private bool OnCanMoveUp(object obj) => _history.CanMoveUp;
        #endregion

        #region Search
        public void Search(object parameter)
        {
            if (parameter != null)
            {
                DirectoriesAndFiles.Clear();
                FilePath = "Результаты поиска в " + FilePath;
                foreach (var item in ItemsToSearch)
                {
                    if (item.Name.Contains(parameter.ToString()))
                    {
                        DirectoriesAndFiles.Add(item);
                    }
                }
                _history.Add(FilePath, Name);
            }
            else throw new ArgumentNullException(nameof(parameter));
        }
        #endregion
    }
}