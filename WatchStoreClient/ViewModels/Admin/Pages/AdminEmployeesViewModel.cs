using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Admin.Pages
{
    public class AdminEmployeesViewModel
    {
        private readonly ApiClient _apiClient;

        public ObservableCollection<ApiClient.EmployeeInfo> Employees { get; } = new();

        public AdminEmployeesViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5148/");
            LoadEmployeesCommand = new RelayCommand(async _ => await LoadEmployees());

            if (!_isLoading)
            {
                _ = LoadEmployees();
            }

        }

        public async Task InitializeAsync()
        {
            await LoadEmployees();
        }
        public ICommand LoadEmployeesCommand { get; }
        public ICommand AddEmployeeCommand { get; }


        private bool _isLoading = false;

        public async Task LoadEmployees()
        {
            if (_isLoading) return;

            try
            {
                _isLoading = true;

                Employees.Clear();

                var employees = await _apiClient.GetEmployeesAsync();

                if (employees != null)
                {
                    foreach (var emp in employees)
                    {
                        if (!Employees.Any(e => e.Id == emp.Id))
                        {
                            Employees.Add(emp);
                        }
                    }
                }
            }
            finally
            {
                _isLoading = false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            var isDeleted = await _apiClient.DeleteEmployeeAsync(employeeId);

            if (isDeleted)
            {
                var employeeToRemove = Employees.FirstOrDefault(e => e.Id == employeeId);
                if (employeeToRemove != null)
                {
                    Employees.Remove(employeeToRemove);
                }
            }

            return isDeleted;
        }


    }
}
