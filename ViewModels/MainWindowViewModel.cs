﻿using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace FileExplorer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        #region Main Panel
        private ObservableCollection<DirectoryItemViewModel> directoryItems = new();
        public ObservableCollection<DirectoryItemViewModel> DirectoryItems
        {
            get => directoryItems;
            set
            {
                directoryItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DirectoryItems)));
            }
        }

        private DirectoryItemViewModel currentDirectoryItem = new();
        public DirectoryItemViewModel CurrentDirectoryItem
        {
            get => currentDirectoryItem;
            set
            {
                currentDirectoryItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentDirectoryItem)));
            }
        }
        #endregion        

        #region Tree View
        private ObservableCollection<Node> nodes;

        public ObservableCollection<Node> Nodes
        {
            get => nodes;
            set { this.RaiseAndSetIfChanged(ref nodes, value); }
        }

        public ObservableCollection<Node> GetChildren(string path)
        {
            ObservableCollection<Node> children = new ObservableCollection<Node>();

            try
            {
                foreach (var subfolders in Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly))
                {
                    if ((File.GetAttributes(subfolders) & (FileAttributes.System | FileAttributes.Hidden))
                       != (FileAttributes.System | FileAttributes.Hidden))
                    {
                        children.Add(new Node { Data = subfolders});
                        children[children.Count - 1].Nodes = new ObservableCollection<Node>(new Node[]
                        {
                            new Node
                            {
                                Data = "123",
                            }
                        }
                        );
                    }
                    //GetChildren(subfolders); //висит
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (DirectoryNotFoundException) { }
            
            return children;
        }
        #endregion

        public MainWindowViewModel()
        {
            DirectoryItems.Add(new DirectoryItemViewModel());

            nodes = new ObservableCollection<Node>();
            foreach (var logicaldrive in Directory.GetLogicalDrives())
            {
                nodes.Add(new Node
                {                    
                    Data = logicaldrive.ToString(),
                    Nodes = GetChildren(logicaldrive.ToString()),
                }
                );
            }
        }
    }
}