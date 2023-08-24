using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

using FileExplorer.ViewModels;
using FileExplorer.ViewModels.Pages;
using FileExplorer.Views.Pages;

using ReactiveUI;

using System.ComponentModel;
using System.IO;

namespace FileExplorer.Views
{
    public partial class MainWindow : Window
    {
        public static bool asc = true; 
        public static bool desc = false;

        public MainWindow()
        {
            InitializeComponent();            
            DataContext = new MainWindowViewModel(new SynchronizationHelper());

            this.Tapped += ClearSearchTextBox;
            //ReplaceCommand = new DelegateCommand(Replace);
        }

        #region Views
        public void SwitchToTabsView(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = true; tabs.IsEnabled = true;
            tiles.IsVisible = false; tiles.IsEnabled = false;
            list.IsVisible = false; list.IsEnabled = false;
            small_icons.IsVisible = false; small_icons.IsEnabled = false;
            regular_icons.IsVisible = false; regular_icons.IsEnabled = false;
            large_icons.IsVisible = false; large_icons.IsEnabled = false;
            largest_icons.IsEnabled = false; largest_icons.IsVisible = false;
            content_view.IsVisible = false; content_view.IsEnabled = false;
        }
        public void SwitchToTilesView(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false; tabs.IsEnabled = false;
            tiles.IsVisible = true; tiles.IsEnabled = true;
            list.IsVisible = false; list.IsEnabled = false;
            small_icons.IsVisible = false; small_icons.IsEnabled = false;
            regular_icons.IsVisible = false; regular_icons.IsEnabled = false;
            large_icons.IsVisible = false; large_icons.IsEnabled = false;
            largest_icons.IsVisible = false; largest_icons.IsEnabled = false;
            content_view.IsVisible = false; content_view.IsEnabled = false;
        }
        public void SwitchToListView(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false; tabs.IsEnabled = false;
            tiles.IsVisible = false; tiles.IsEnabled = false;
            list.IsVisible = true; list.IsEnabled = true;
            small_icons.IsVisible = false; small_icons.IsEnabled = false;
            regular_icons.IsVisible = false; regular_icons.IsEnabled = false;
            large_icons.IsVisible = false; large_icons.IsEnabled = false;
            largest_icons.IsVisible = false; largest_icons.IsEnabled = false;
            content_view.IsVisible = false; content_view.IsEnabled = false;
        }
        public void SwitchToSmallIcons(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false; tabs.IsEnabled = false;
            tiles.IsVisible = false; tiles.IsEnabled = false;
            list.IsVisible = false; list.IsEnabled = false;
            small_icons.IsVisible = true; small_icons.IsEnabled = true;
            regular_icons.IsVisible = false; regular_icons.IsEnabled = false;
            large_icons.IsVisible = false; large_icons.IsEnabled = false;
            largest_icons.IsVisible = false; largest_icons.IsEnabled = false;
            content_view.IsVisible = false; content_view.IsEnabled = false;
        }
        public void SwitchToRegularIcons(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false; tabs.IsEnabled = false;
            tiles.IsVisible = false; tiles.IsEnabled = false;
            list.IsVisible = false; list.IsEnabled = false;
            small_icons.IsVisible = false; small_icons.IsEnabled = false;
            regular_icons.IsVisible = true; regular_icons.IsEnabled = true;
            large_icons.IsVisible = false; large_icons.IsEnabled = false;
            largest_icons.IsVisible = false; largest_icons.IsEnabled = false;
            content_view.IsVisible = false; content_view.IsEnabled = false;
        }
        public void SwitchToLargeIcons(object sender, RoutedEventArgs routedEventArgs)  
        {
            tabs.IsVisible = false; tabs.IsEnabled = false;
            tiles.IsVisible = false; tiles.IsEnabled = false;
            list.IsVisible = false; list.IsEnabled = false;
            small_icons.IsVisible = false; small_icons.IsEnabled = false;
            regular_icons.IsVisible = false; regular_icons.IsEnabled = false;
            large_icons.IsVisible = true; large_icons.IsEnabled = true;
            largest_icons.IsVisible = false; largest_icons.IsEnabled = false;
            content_view.IsVisible = false; content_view.IsEnabled = false;
        }
        public void SwitchToLargestIcons(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false; tabs.IsEnabled = false;
            tiles.IsVisible = false; tiles.IsEnabled = false;
            list.IsVisible = false; list.IsEnabled = false;
            small_icons.IsVisible = false; small_icons.IsEnabled = false;
            regular_icons.IsVisible = false; regular_icons.IsEnabled = false;
            large_icons.IsVisible = false; large_icons.IsEnabled = false;
            largest_icons.IsVisible = true; largest_icons.IsEnabled = true;
            content_view.IsVisible = false; content_view.IsEnabled = false;
        }
        public void SwitchToContent(object sender, RoutedEventArgs routedEventArgs)
        {
            tabs.IsVisible = false; tabs.IsEnabled = false;
            tiles.IsVisible = false; tiles.IsEnabled = false;
            list.IsVisible = false; list.IsEnabled = false;
            small_icons.IsVisible = false; small_icons.IsEnabled = false;
            regular_icons.IsVisible = false; regular_icons.IsEnabled = false;
            large_icons.IsVisible = false; large_icons.IsEnabled = false;
            largest_icons.IsVisible = false; largest_icons.IsEnabled = false;
            content_view.IsVisible = true; content_view.IsEnabled = true;
        }

        #endregion

        #region Panels
        public void NavigationPanel(object sender, RoutedEventArgs routedEventArgs)
        {
            if (quick_access.IsVisible == true && tree.IsVisible == true)
            {
                tree.IsVisible = false;
                quick_access.IsVisible = false;

                main_panel.SetValue(Grid.ColumnProperty, 0);
                if (info.IsVisible == true)
                {
                    main_panel.SetValue(Grid.ColumnSpanProperty, 4);
                }
                else
                {
                    main_panel.SetValue(Grid.ColumnSpanProperty, 6);
                }              
            }
            else
            {
                tree.IsVisible = true;
                quick_access.IsVisible = true;

                main_panel.SetValue(Grid.ColumnProperty, 3);
                if (info.IsVisible == true)
                {
                    main_panel.SetValue(Grid.ColumnSpanProperty, 1);
                }
                else
                {
                    main_panel.SetValue(Grid.ColumnSpanProperty, 3);
                }
                
            }
        }
        public void InformationPanel(object sender, RoutedEventArgs routedEventArgs)
        {
            if (info.IsVisible == false)
            {
                info.IsVisible = true;
                if (quick_access.IsVisible == true && tree.IsVisible == true)
                {
                    main_panel.SetValue(Grid.ColumnSpanProperty, 1);
                }
                else
                {
                    main_panel.SetValue(Grid.ColumnSpanProperty, 4);
                }                
            }
            else
            {
                info.IsVisible = false;
                if (quick_access.IsVisible == true && tree.IsVisible == true)
                {
                    main_panel.SetValue(Grid.ColumnSpanProperty, 3);
                }
                else
                {
                    main_panel.SetValue(Grid.ColumnSpanProperty, 6);
                }
            }
        }
        #endregion

        #region Menu
        public void ChangeSortingModeToName(object sender, RoutedEventArgs routedEventArgs)
        {
            sort_name.IsChecked = true; context_sort_name.IsChecked = true; 
            sort_date.IsChecked = false; context_sort_date.IsChecked = false;
            sort_type.IsChecked = false; context_sort_type.IsChecked = false; 
            sort_size.IsChecked = false; context_sort_size.IsChecked = false;
        }
        public void ChangeSortingModeToDateOfChange(object sender, RoutedEventArgs routedEventArgs)
        {
            sort_name.IsChecked = false; context_sort_name.IsChecked = false; 
            sort_date.IsChecked = true; context_sort_date.IsChecked = true; 
            sort_type.IsChecked = false; context_sort_type.IsChecked = false; 
            sort_size.IsChecked = false; context_sort_size.IsChecked = false; 
        }
        public void ChangeSortingModeToType(object sender, RoutedEventArgs routedEventArgs)
        {
            sort_name.IsChecked = false; context_sort_name.IsChecked = false; 
            sort_date.IsChecked = false; context_sort_date.IsChecked = false; 
            sort_type.IsChecked = true; context_sort_type.IsChecked = true; 
            sort_size.IsChecked = false; context_sort_size.IsChecked = false; 
        }
        public void ChangeSortingModeToSize(object sender, RoutedEventArgs routedEventArgs)
        {
            sort_name.IsChecked = false; context_sort_name.IsChecked = false; 
            sort_date.IsChecked = false; context_sort_date.IsChecked = false; 
            sort_type.IsChecked = false; context_sort_type.IsChecked = false; 
            sort_size.IsChecked = true; context_sort_size.IsChecked = true; 
        }
        
        public void ChangeSortingModeToAscending(object sender, RoutedEventArgs routedEventArgs)
        {
            ascending.IsChecked = true; context_ascending.IsChecked = true; asc = true;
            descending.IsChecked = false; context_descending.IsChecked = false; desc = false;

        }
        public void ChangeSortingModeToDescending(object sender, RoutedEventArgs routedEventArgs)
        {
            ascending.IsChecked = false; context_ascending.IsChecked = false; asc = false;
            descending.IsChecked = true; context_descending.IsChecked = true; desc = true;
        }
        #endregion

        #region PropertiesWindow
        public void ShowPropertiesWindow(object sender, RoutedEventArgs routedEventArgs)
        {
            var window = new PropertiesWindow();
            
            window.Show(this);
            
            //window.Show();
        }
        #endregion

        #region Search
        private void ClearSearchTextBox(object sender, RoutedEventArgs routedEventArgs) 
        {
            searchBox.Clear();
        }
        #endregion

        #region Selection

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

        #endregion

        //private async void OpenFolderDialogButtonClick(object sender, RoutedEventArgs routedEventArgs)
        //{
        //    OpenFolderDialog openFolderDialog = new OpenFolderDialog();
        //    string? result = await openFolderDialog.ShowAsync(this);
        //    if (DataContext is MainWindowViewModel dataContext)
        //    {
        //        if (result != null)
        //        {
        //            dataContext.Path = result;
        //            DirectoryInfo directoryInfo = new DirectoryInfo(result);

        //        }
        //        else
        //        {
        //            dataContext.Path = "Dialog was canceled";
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



    }
}