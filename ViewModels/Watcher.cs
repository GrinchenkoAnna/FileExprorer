using System.IO;
using System.Threading.Tasks;

namespace FileExplorer.ViewModels
{
    public class Watcher : DirectoryItemViewModel
    {
        private readonly ISynchronizationHelper _synchronizationHelper;
        public Watcher(ISynchronizationHelper synchronizationHelper, string filePath) : base(synchronizationHelper)
        {
            _synchronizationHelper = synchronizationHelper;
            FilePath = filePath;
        }

        public DirectoryInfo StartWatcher()
        {
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

            return directoryInfo;
        }

        private async void OnCreated(object sender, FileSystemEventArgs e)
        {
            await Task.Run(() =>
            {
                _synchronizationHelper.InvokeAsync(async () =>
                {
                    await AddItemFromSystem(e);
                });
            });
        }
        private Task AddItemFromSystem(FileSystemEventArgs e)
        {
            FileAttributes attr = System.IO.File.GetAttributes(e.FullPath);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                foreach (var item in DirectoriesAndFiles)
                {
                    if (item.FullName == e.FullPath) { return Task.CompletedTask; }
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
                        if (item.FullName == e.FullPath) { return Task.CompletedTask; }
                    }
                    FileViewModel newFile = new FileViewModel(e.Name);
                    DirectoriesAndFiles.Add(newFile);
                    newFile.FullName = e.FullPath;
                }
            }

            return Task.CompletedTask;
        }

        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            await Task.Run(() =>
            {
                _synchronizationHelper.InvokeAsync(async () =>
                {
                    await ChangedItemFromSystem(e);
                });
            });
        }
        private Task ChangedItemFromSystem(FileSystemEventArgs e)
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

            return Task.CompletedTask;
        }

        private async void OnDeleted(object sender, FileSystemEventArgs e)
        {
            await Task.Run(() =>
            {
                _synchronizationHelper.InvokeAsync(async () =>
                {
                    await DeletedItemFromSystem(e);
                });
            });
        }
        private async Task DeletedItemFromSystem(FileSystemEventArgs e)
        {
            foreach (var collection in Collections)
            {
                foreach (var item in collection)
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
                _synchronizationHelper.InvokeAsync(async () =>
                {
                    await RenamedItemFromSystem(e);
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
    }
}