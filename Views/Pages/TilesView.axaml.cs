using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

using FileExplorer.ViewModels;

using System.Collections.Generic;

namespace FileExplorer.Views.Pages
{
    public partial class TilesView : UserControl
    {
        public DelegateCommand SelectFirstItemCommand { get; }
        public TilesView()
        {
            InitializeComponent();
            this.KeyDown += SelectFirstItem;
        }

        private void SelectFirstItem(object sender, KeyEventArgs keyEventArgs)
        {
            if (content.SelectedItems.Count != 0)
            {
                content.SelectedItems.Clear();
            }
            else
            {
                content.SelectedIndex = 0;
            }
        }
    }
}
