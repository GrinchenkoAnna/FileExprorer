using Avalonia.Controls;
using Avalonia.Input;

using FileExplorer.ViewModels;

using System;

namespace FileExplorer.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}