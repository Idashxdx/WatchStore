using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WatchStoreClient.ViewModels.Admin.Pages;

namespace WatchStoreClient.Views.Admin.Pages
{
    public partial class AdminBackupsView : UserControl
    {
        public AdminBackupsView()
        {
            InitializeComponent();
            DataContext = new AdminBackupsViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

