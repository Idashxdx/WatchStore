using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using System;
using WatchStoreClient.Services;
using WatchStoreClient.ViewModels.Employee.Pages;

namespace WatchStoreClient.Views.Employee.Pages
{
    public partial class EmployeeWatchesView : UserControl
    {
        public EmployeeWatchesView()
        {
            InitializeComponent();

           DataContext = new EmployeeWatchesViewModel();
        }

        private async void OnAddWatchButtonClick(object? sender, RoutedEventArgs e)
        {
            var parentWindow = this.GetVisualRoot() as Window;
            if (parentWindow == null)
            {
                Console.WriteLine("Не удалось получить родительское окно для отображения диалога.");
                return;
            }

            var addWatchWindow = new AddWatchWindow();

            addWatchWindow.DataContext = new AddWatchViewModel(() =>
            {
                if (DataContext is EmployeeWatchesViewModel viewModel)
                {
                    _ = viewModel.LoadWatchesAsync();
                }
                addWatchWindow.Close();
            });

            await addWatchWindow.ShowDialog(parentWindow);
        }


        private async void OnDeleteClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int watchId)
            {
               if (DataContext is EmployeeWatchesViewModel viewModel)
                {
                    var result = await viewModel.RemoveWatchAsync(watchId);

                    if (!result)
                    {
                        Console.WriteLine($"Ошибка удаления часов с ID: {watchId}");
                    }
                }
            }
        }
        private async void OnAddImageClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int watchId)
            {
                var parentWindow = this.GetVisualRoot() as Window;
                if (parentWindow == null) return;

               var addWatchImageWindow = new AddWatchImageWindow
                {
                    DataContext = new AddWatchImageViewModel(watchId)
                };

                await addWatchImageWindow.ShowDialog(parentWindow);
            }
        }

    }
}