using Avalonia;
using Avalonia.Threading;

using System;
using System.Threading.Tasks;
using System.Windows;

using static FileExplorer.ViewModels.DirectoryItemViewModel;

namespace FileExplorer.ViewModels
{
    internal class SynchronizationHelper : ISynchronizationHelper
    {
        public async Task InvokeAsync(Action action) => await Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Background);
    }
}
