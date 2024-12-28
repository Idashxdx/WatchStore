// OrderWindow.xaml.cs
using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WatchStoreClient.ViewModels.Client.Pages;

namespace WatchStoreClient.Views.Client
{
    public partial class OrderWindow : Window
    {
        public OrderWindow(OrderWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            if (viewModel != null)
            {
                viewModel.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(OrderWindowViewModel.StatusMessage))
                    {
                        var statusMessage = viewModel.StatusMessage;
                        if (statusMessage == "Заказ успешно оформлен!")
                        {
                            this.Close(); 
                        }
                    }
                };
            }
        }
        private void CloseOrderWindow(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close();
        }

    }

}
