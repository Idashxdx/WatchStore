using System;
using Avalonia.Controls;
using Avalonia.VisualTree;
using WatchStoreClient.Services;
using WatchStoreClient.ViewModels.Client.Pages;

namespace WatchStoreClient.Views.Client.Pages
{
    public partial class ClientWatchesView : UserControl
    {
        public ClientWatchesView()
        {
            InitializeComponent();
        }

        private void OnOrderButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (DataContext is ClientWatchesViewModel viewModel &&
                sender is Button button &&
                button.Tag is ApiClient.Watch selectedWatch)
            {
                var parentWindow = this.GetVisualRoot() as Window;
                if (parentWindow == null) return;

                var orderWindow = new OrderWindow(
                    new OrderWindowViewModel(
                        selectedWatch,
                        viewModel.UserId,
                        async () =>
                        {
                            await viewModel.ReloadWatches(); 
                        }
                    )
                );

                orderWindow.ShowDialog(parentWindow); 
            }
        }


    }
}
