using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using WatchStoreClient.ViewModels.Admin.Pages;

namespace WatchStoreClient.Views.Admin.Pages
{
    public partial class AdminEmployeesView : UserControl
    {
        public AdminEmployeesView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void OnDeleteClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int employeeId)
            {
                if (DataContext is AdminEmployeesViewModel viewModel)
                {
                    var result = await viewModel.DeleteEmployeeAsync(employeeId);

                    if (!result)
                    {
                        Console.WriteLine($"Ошибка удаления сотрудника с ID: {employeeId}");
                    }
                }
            }
        }

        private async void OnAddEmployeeClick(object? sender, RoutedEventArgs e)
        {
            var parentWindow = this.GetVisualRoot() as Window;
            if (parentWindow == null) return;

            var addEmployeeWindow = new AddEmployeeWindow();

            addEmployeeWindow.DataContext = new AddEmployeeWindowViewModel(async () =>
            {
                if (DataContext is AdminEmployeesViewModel viewModel)
                {
                    await viewModel.LoadEmployees(); 
                }

                addEmployeeWindow.Close(); 
            });

            await addEmployeeWindow.ShowDialog(parentWindow);
        }

    }
}
