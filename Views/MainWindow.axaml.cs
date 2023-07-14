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
            small_icons.IsVisible = false;
            regular_icons.IsVisible = false;
            large_icons.IsVisible = false;
            largest_icons.IsVisible = false;
            content_view.IsVisible = false;
        }

        public void SwitchToTilesView(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false;
            tiles.IsVisible = true;
            list.IsVisible = false;
            small_icons.IsVisible = false;
            regular_icons.IsVisible = false;
            large_icons.IsVisible = false;
            largest_icons.IsVisible = false;
            content_view.IsVisible = false;
        }

        public void SwitchToListView(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false;
            tiles.IsVisible = false;
            list.IsVisible = true;
            small_icons.IsVisible = false;
            regular_icons.IsVisible = false;
            large_icons.IsVisible = false;
            largest_icons.IsVisible = false;
            content_view.IsVisible = false;
        }

        public void SwitchToSmallIcons(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false;
            tiles.IsVisible = false;
            list.IsVisible = false;
            small_icons.IsVisible = true;
            regular_icons.IsVisible = false;
            large_icons.IsVisible = false;
            largest_icons.IsVisible = false;
            content_view.IsVisible = false;
        }

        public void SwitchToRegularIcons(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false;
            tiles.IsVisible = false;
            list.IsVisible = false;
            small_icons.IsVisible = false;
            regular_icons.IsVisible = true;
            large_icons.IsVisible = false;
            largest_icons.IsVisible = false;
            content_view.IsVisible = false;
        }

        public void SwitchToLargeIcons(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false;
            tiles.IsVisible = false;
            list.IsVisible = false;
            small_icons.IsVisible = false;
            regular_icons.IsVisible = false;
            large_icons.IsVisible = true;
            largest_icons.IsVisible = false;
            content_view.IsVisible = false;
        }

        public void SwitchToLargestIcons(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false;
            tiles.IsVisible = false;
            list.IsVisible = false;
            small_icons.IsVisible = false;
            regular_icons.IsVisible = false;
            large_icons.IsVisible = false;
            largest_icons.IsVisible = true;
            content_view.IsVisible = false;
        }

        public void SwitchToContent(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false;
            tiles.IsVisible = false;
            list.IsVisible = false;
            small_icons.IsVisible = false;
            regular_icons.IsVisible = false;
            large_icons.IsVisible = false;
            largest_icons.IsVisible = false;
            content_view.IsVisible = true;
        }

        public void NavigationPanel(object sender, RoutedEventArgs routedEventArgs)
        {
            if (quick_access.IsVisible == true && tree.IsVisible == true)
            {
                tree.IsVisible = false;
                quick_access.IsVisible = false;

                main_panel.SetValue(Grid.ColumnProperty, 0);
                main_panel.SetValue(Grid.ColumnSpanProperty, 5);
                background.SetValue(Grid.ColumnProperty, 0);
                background.SetValue(Grid.ColumnSpanProperty, 5);
                background.Margin = new Avalonia.Thickness(0,0,0,0);
            }
            else
            {
                tree.IsVisible = true;
                quick_access.IsVisible = true;

                main_panel.SetValue(Grid.ColumnProperty, 3);
                main_panel.SetValue(Grid.ColumnSpanProperty, 2);
                background.SetValue(Grid.ColumnProperty, 3);
                background.SetValue(Grid.ColumnSpanProperty, 2);
                background.Margin = new Avalonia.Thickness(2, 0, 0, 0);
            }
        }

        public void InformationPanel(object sender, RoutedEventArgs routedEventArgs)
        {
            if (info.IsVisible == false)
            {
                info.IsVisible = true;
            }
            else
            {
                info.IsVisible = false;
            }
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