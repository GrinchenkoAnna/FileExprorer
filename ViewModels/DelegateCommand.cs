﻿using Avalonia.Input;

using System;
using System.Windows.Input;

namespace FileExplorer.ViewModels
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute = null) 
        { 
            _execute = execute;
            this._canExecute = canExecute;
        }       

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute.Invoke(parameter);            
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged() 
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}