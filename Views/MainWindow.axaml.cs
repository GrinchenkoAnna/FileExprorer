using Avalonia.Controls;
using FileExplorer.ViewModels;

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