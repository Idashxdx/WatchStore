using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Employee.Pages
{
    public class EmployeeRequestsViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient = new("http://localhost:5148/");
        private int _employeeId;

        private string _statusMessage = string.Empty;
        private string _filterText = string.Empty;

        public ObservableCollection<ApiClient.UserRequest> Requests { get; } = new ObservableCollection<ApiClient.UserRequest>();


        public string StatusMessage
        {
            get => _statusMessage;
            private set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public ICommand UpdateRequestStatusCommand { get; }

        public EmployeeRequestsViewModel(int userId)
        {
            LoadEmployeeAndRequests(userId);

            UpdateRequestStatusCommand = new RelayCommand(async param =>
            {
                if (param is ApiClient.UserRequest request)
                {
                    await UpdateRequestStatusAsync(request.RequestId, (int)RequestStatus.ВОбработке);
                }
            });
        }

        private async void LoadEmployeeAndRequests(int userId)
        {
            _employeeId = await _apiClient.GetEmployeeIdByUserIdAsync(userId) ?? 0;

            if (_employeeId == 0)
            {
                StatusMessage = "Ошибка: Не удалось найти ID сотрудника.";
            }
            else
            {
                Console.WriteLine($"[DEBUG] Загружен EmployeeId: {_employeeId}");
                await LoadRequestsAsync();
            }
        }


        public async Task LoadRequestsAsync()
        {
            var requests = await _apiClient.GetRequestsAsync();
            if (requests != null)
            {
                foreach (var request in requests)
                {
                    var existing = Requests.FirstOrDefault(r => r.RequestId == request.RequestId);
                    if (existing != null)
                    {
                        var index = Requests.IndexOf(existing);
                        Requests[index] = request;
                    }
                    else
                    {
                        Requests.Add(request);
                    }
                }

                for (int i = Requests.Count - 1; i >= 0; i--)
                {
                    if (!requests.Any(r => r.RequestId == Requests[i].RequestId))
                    {
                        Requests.RemoveAt(i);
                    }
                }
            }
        }




        public async Task<bool> UpdateRequestStatusAsync(int requestId, int newStatusId)
        {
            if (_employeeId == 0)
            {
                StatusMessage = "Ошибка: ID сотрудника не найден.";
                return false;
            }

            var success = await _apiClient.UpdateRequestStatusAsync(requestId, newStatusId, _employeeId);
            if (success)
            {
                StatusMessage = "Статус заявки успешно обновлён!";

               var updated = await _apiClient.GetRequestsAsync();
                var updatedRequest = updated?.FirstOrDefault(r => r.RequestId == requestId);
                if (updatedRequest != null)
                {
                    var existing = Requests.FirstOrDefault(r => r.RequestId == requestId);
                    if (existing != null)
                    {
                        var index = Requests.IndexOf(existing);
                        Requests[index] = updatedRequest;
                    }
                }
            }
            else
            {
                StatusMessage = "Ошибка: Не удалось обновить статус заявки.";
            }
            return success;
        }

        private readonly ObservableCollection<ApiClient.UserRequest> _allRequests = new();

        private void ApplyFilter()
        {
            if (string.IsNullOrEmpty(_filterText))
            {
                return;
            }

            for (int i = Requests.Count - 1; i >= 0; i--)
            {
                var request = Requests[i];

                if (!request.Description.Contains(_filterText, StringComparison.OrdinalIgnoreCase))
                {
                    Requests.RemoveAt(i);
                }
            }

            var filteredItems = _allRequests.Where(r => r.Description.Contains(_filterText, StringComparison.OrdinalIgnoreCase));

            foreach (var item in filteredItems)
            {
                if (!Requests.Contains(item))
                {
                    Requests.Add(item);
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public enum RequestStatus
        {
            Новый = 1,
            ВОбработке = 2,
            Завершён = 3,
            Закрыто = 4
        }

    }

}
