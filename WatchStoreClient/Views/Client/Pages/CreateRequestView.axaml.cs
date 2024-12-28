using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WatchStoreClient.ViewModels.Client.Pages;

namespace WatchStoreClient.Views.Client.Pages
{
    public partial class CreateRequestView : UserControl
    {
        public CreateRequestView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public CreateRequestView(CreateRequestViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}

