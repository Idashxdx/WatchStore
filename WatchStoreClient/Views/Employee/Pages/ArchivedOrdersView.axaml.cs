using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WatchStoreClient.ViewModels.Employee.Pages;

namespace WatchStoreClient.Views.Employee.Pages
{
    public partial class ArchivedOrdersView : UserControl
    {
        public ArchivedOrdersView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public ArchivedOrdersView(ArchivedOrdersViewModel viewModel) : this()
        {
            DataContext = viewModel; 

        }
    }
}
