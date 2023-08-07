using Avalonia.Controls;

using FileExplorer.ViewModels;

namespace FileExplorer.Views
{
    public partial class PropertiesWindow : Window
    {
        public PropertiesWindow()
        {
            InitializeComponent();
            //DataContext = new MainWindowViewModel(new SynchronizationHelper());
        }
    }
}
