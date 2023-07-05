using Avalonia.Controls;
using Avalonia.Interactivity;

using FileExplorer.ViewModels;

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

        private void SwitchToTabView(object sender, RoutedEventArgs routedEventArgs)
        {
            MainWindowViewModel.TabsViewModelVisible = true;
            MainWindowViewModel.TilesViewModelVisible = false;
        }

        private void SwitchToTilesView(object sender, RoutedEventArgs routedEventArgs)
        {
            MainWindowViewModel.TabsViewModelVisible = false;
            MainWindowViewModel.TilesViewModelVisible = true;
        }

        //private void SelectAllItems(object sender, RoutedEventArgs routedEventArgs) 
        //{
        //    foreach (var item in listbox.Items)
        //    {
        //        listbox.SelectedItems.Add(item);
        //    }
        //}

        //private void DeselectAllItems(object sender, RoutedEventArgs routedEventArgs)
        //{
        //    listbox.SelectedItems.Clear();
        //}

        //private void ReverseItemsSelection(object sender, RoutedEventArgs routedEventArgs)
        //{
        //    foreach (var item in listbox.Items)
        //    {
        //        if (listbox.SelectedItems.Contains(item))
        //        {
        //            listbox.SelectedItems.Remove(item);
        //        }
        //        else
        //        {
        //            listbox.SelectedItems.Add(item);
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