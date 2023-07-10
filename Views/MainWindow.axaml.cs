using Avalonia.Controls;
using Avalonia.Interactivity;

using FileExplorer.ViewModels;
using FileExplorer.ViewModels.Pages;
using FileExplorer.Views.Pages;

using ReactiveUI;

using System.ComponentModel;

namespace FileExplorer.Views
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();            
            DataContext = new MainWindowViewModel(new SynchronizationHelper());          

            //ReplaceCommand = new DelegateCommand(Replace);
        }

        public void SwitchToTabsView(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = true;
            tiles.IsVisible = false;
            list.IsVisible = false;
        }

        public void SwitchToTilesView(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false;
            tiles.IsVisible = true;
            list.IsVisible = false;
        }

        public void SwitchToListView(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false;
            tiles.IsVisible = false;
            list.IsVisible = true;
        }

        //private void SelectAllItems(object sender, RoutedEventArgs routedEventArgs)
        //{            
        //    foreach (var item in content.Items)
        //    {
        //        content.SelectedItems.Add(item);
        //    }
        //}

        //private void DeselectAllItems(object sender, RoutedEventArgs routedEventArgs)
        //{
        //    content.SelectedItems.Clear();
        //}

        //private void ReverseItemsSelection(object sender, RoutedEventArgs routedEventArgs)
        //{
        //    foreach (var item in content.Items)
        //    {
        //        if (content.SelectedItems.Contains(item))
        //        {
        //            content.SelectedItems.Remove(item);
        //        }
        //        else
        //        {
        //            content.SelectedItems.Add(item);
        //        }
        //    }
        //}

        //public DelegateCommand ReplaceCommand { get; }

        //private void Replace(object parameter, )
        //{

        //}

        //private async void OpenFileDialogButtonClick(object sender, RoutedEventArgs routedEventArgs)
        //{
        //    if (sender is FileEntityViewModel)
        //    {
        //        OpenFileDialog openFileDialog = new OpenFileDialog();
        //        //openFileDialog.AllowMultiple = true;
        //        string[]? result = await openFileDialog.ShowAsync(this);
        //        if (DataContext is MainWindowViewModel dataContext)
        //        {
        //            if (result != null)
        //            {
        //                dataContext.Path = string.Join(';', result);
        //            }
        //            else
        //            {
        //                dataContext.Path = "Dialog was canceled";
        //            }
        //        }
        //    }            
        //}
        //private async void SaveFileDialogButtonClick(object sender, RoutedEventArgs routedEventArgs)
        //{
        //    SaveFileDialog saveFileDialog = new SaveFileDialog();
        //    string? result = await saveFileDialog.ShowAsync(this);
        //    if (DataContext is MainWindowViewModel dataContext)
        //    {
        //        if (result != null)
        //        {
        //            dataContext.Path = result;
        //        }
        //        else
        //        {
        //            dataContext.Path = "Dialog was canceled";
        //        }
        //    }
        //}
        private async void OpenFolderDialogButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            string? result = await openFolderDialog.ShowAsync(this);
            if (DataContext is MainWindowViewModel dataContext)
            {
                if (result != null)
                {
                    dataContext.Path = result;
                }
                else
                {
                    dataContext.Path = "Dialog was canceled";
                }
            }              
        }
    }
}