using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using WatchStoreClient.ViewModels.Employee.Pages;

namespace WatchStoreClient.Views.Employee.Pages
{
    public partial class EmployeeOrdersView : UserControl
    {
        public EmployeeOrdersView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void OnProcessClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                if (DataContext is EmployeeOrdersViewModel viewModel)
                {
                    bool success = await viewModel.UpdateOrderStatusAsync(orderId, (int)EmployeeOrdersViewModel.OrderStatus.Обрабатывается);
                    if (success)
                    {
                        await viewModel.LoadOrdersAsync(); 
                    }
                }
            }
        }

        private async void OnShippedClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                if (DataContext is EmployeeOrdersViewModel viewModel)
                {
                    bool success = await viewModel.UpdateOrderStatusAsync(orderId, (int)EmployeeOrdersViewModel.OrderStatus.Отправлен);
                    if (success)
                    {
                        await viewModel.LoadOrdersAsync();
                    }
                }
            }
        }

        private async void OnDeliveredClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                if (DataContext is EmployeeOrdersViewModel viewModel)
                {
                    bool success = await viewModel.UpdateOrderStatusAsync(orderId, (int)EmployeeOrdersViewModel.OrderStatus.Доставлен);
                    if (success)
                    {
                        await viewModel.LoadOrdersAsync();
                    }
                }
            }
        }

        private async void OnCancelledClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                if (DataContext is EmployeeOrdersViewModel viewModel)
                {
                    bool success = await viewModel.UpdateOrderStatusAsync(orderId, (int)EmployeeOrdersViewModel.OrderStatus.Отменён);
                    if (success)
                    {
                        await viewModel.LoadOrdersAsync();
                    }
                }
            }
        }
    }
}
