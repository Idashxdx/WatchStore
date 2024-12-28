using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WatchStoreClient.ViewModels.Admin;

namespace WatchStoreClient.Views.Admin
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();

                  DataContext = new AdminWindowViewModel(OnLogout); 
        }

        private void OnLogout()
        {
            new MainWindow().Show();
            Close();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
