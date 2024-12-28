using System;
using Avalonia.Controls;
using WatchStoreClient.ViewModels.Client;

namespace WatchStoreClient.Views.Client
{
    public partial class ClientWindow : Window
    {
        public ClientWindow(int userId)
        {
            InitializeComponent();
         
            DataContext = new ClientWindowViewModel(userId, () =>
            {
                new MainWindow().Show();
                this.Close();
            });
        }
    }
}
