using System;
using Avalonia.Controls;
using WatchStoreClient.ViewModels.Guest;

namespace WatchStoreClient.Views.Guest
{
    public partial class GuestWindow : Window
    {
        public GuestWindow(Action onLogoutCallback)
        {
            InitializeComponent();
            DataContext = new GuestWindowViewModel(() =>
            {
                onLogoutCallback.Invoke();
                this.Close();
            });
        }

    }
}
