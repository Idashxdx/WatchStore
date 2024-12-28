using Avalonia.Controls;
using Avalonia.Interactivity;
using WatchStoreClient.ViewModels.Employee.Pages;

namespace WatchStoreClient.Views.Employee.Pages
{
    public partial class AddWatchWindow : Window
    {
        public AddWatchWindow()
        {
            InitializeComponent();
        }

        private void OnCancelClick(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
