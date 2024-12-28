using System;
using Avalonia.Controls;
using WatchStoreClient.ViewModels.Employee;

namespace WatchStoreClient.Views.Employee
{
    public partial class EmployeeWindow : Window
    {
        private readonly int _employeeId;
        public EmployeeWindow(int employeeId)
        {
            InitializeComponent();

            _employeeId = employeeId; 
            DataContext = new EmployeeWindowViewModel(employeeId, OnLogout); 
        }

        private void OnLogout()
        {
            new MainWindow().Show(); 
            Close();
        }
    }
}