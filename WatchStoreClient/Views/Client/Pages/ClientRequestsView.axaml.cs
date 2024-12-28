using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WatchStoreClient.ViewModels.Client.Pages;

namespace WatchStoreClient.Views.Client.Pages
{
    public partial class ClientRequestsView : UserControl
    {
        public ClientRequestsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public ClientRequestsView(ClientRequestsViewModel viewModel) : this()
        {
            DataContext = viewModel; 
        }
    }
}

