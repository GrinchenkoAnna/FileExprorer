using Avalonia.Controls;

using FileExplorer.ViewModels;

using System;
using System.IO;

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