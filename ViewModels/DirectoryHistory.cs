﻿using Avalonia.Input;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Concurrency;

namespace FileExplorer.ViewModels
{
    internal class DirectoryHistory : IDirectoryHistory
    {
        public event EventHandler HistoryChanged;
        //public event EventHandler<KeyEventArgs> KeyPress;

        public DirectoryNode Current { get; private set; }

        private DirectoryNode _head;

        public DirectoryHistory(string directoryPath, string directoryPathName)
        {
            _head = new DirectoryNode(directoryPath, directoryPathName);
            Current = _head;
        }

        public IEnumerator<DirectoryNode> GetEnumerator()
        {
            yield return Current;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool CanMoveBack => Current.PreviousNode != null;
        public bool CanMoveForward => Current.NextNode != null;   

        public void Add(string filePath, string name)
        {
            var node  = new DirectoryNode(filePath, name);

            Current.NextNode = node;
            node.PreviousNode = Current;
            Current = node;

            RaiseHistoryChanged();
        }

        public void MoveBack()
        {
            var prev = Current.PreviousNode;
            Current = prev;

            RaiseHistoryChanged();
        }

        public void MoveForward()
        {
            var next = Current.NextNode;
            Current = next;

            RaiseHistoryChanged();
        }

        private void RaiseHistoryChanged() => HistoryChanged?.Invoke(this, EventArgs.Empty);        
    }
}
