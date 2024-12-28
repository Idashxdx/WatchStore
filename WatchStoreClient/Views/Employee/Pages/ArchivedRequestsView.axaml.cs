using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WatchStoreClient.ViewModels.Employee.Pages;

namespace WatchStoreClient.Views.Employee.Pages
{
    public partial class ArchivedRequestsView : UserControl
    {
        public ArchivedRequestsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        public ArchivedRequestsView(ArchivedRequestsViewModel viewModel) : this()
        {
            DataContext = viewModel;

        }
    }
}
