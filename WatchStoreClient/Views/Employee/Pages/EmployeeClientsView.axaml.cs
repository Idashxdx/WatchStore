using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WatchStoreClient.ViewModels.Employee.Pages;

namespace WatchStoreClient.Views.Employee.Pages
{
    public partial class EmployeeClientsView : UserControl
    {
        public EmployeeClientsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        public EmployeeClientsView(EmployeeClientsViewModel viewModel) : this()
        {
            DataContext = viewModel; 

        }
    }
}
