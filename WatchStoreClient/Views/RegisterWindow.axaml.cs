using Avalonia.Controls;
using WatchStoreClient.ViewModels;

namespace WatchStoreClient.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();

            DataContext = new RegisterWindowViewModel();
        }
    }
}
