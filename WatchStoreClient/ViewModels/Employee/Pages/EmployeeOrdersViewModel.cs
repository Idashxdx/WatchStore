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
    public class EmployeeOrdersViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient = new("http://localhost:5148/");
        private int _employeeId;

        private string _statusMessage = string.Empty;

        public ObservableCollection<ApiClient.UserOrder> Orders { get; } = new ObservableCollection<ApiClient.UserOrder>();

        public string StatusMessage
        {
            get => _statusMessage;
            private set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateOrderStatusCommand { get; }

        public EmployeeOrdersViewModel(int userId)
        {
            LoadEmployeeAndOrders(userId);

            UpdateOrderStatusCommand = new RelayCommand(async param =>
            {
                if (param is ApiClient.UserOrder order)
                {
                    await UpdateOrderStatusAsync(order.OrderId, (int)OrderStatus.Обрабатывается);
                }
            });
        }

        private async void LoadEmployeeAndOrders(int userId)
        {
            _employeeId = await _apiClient.GetEmployeeIdByUserIdAsync(userId) ?? 0;

            if (_employeeId == 0)
            {
                StatusMessage = "Ошибка: Не удалось найти ID сотрудника.";
            }
            else
            {
                Console.WriteLine($"[DEBUG] Загружен EmployeeId: {_employeeId}");
                await LoadOrdersAsync();
            }
        }

        public async Task LoadOrdersAsync()
        {
            var orders = await _apiClient.GetOrdersAsync();
            if (orders != null)
            {
                foreach (var order in orders)
                {
                    var existing = Orders.FirstOrDefault(o => o.OrderId == order.OrderId);
                    if (existing != null)
                    {
                        var index = Orders.IndexOf(existing);
                        Orders[index] = order;
                    }
                    else
                    {
                        Orders.Add(order);
                    }
                }

                for (int i = Orders.Count - 1; i >= 0; i--)
                {
                    if (!orders.Any(o => o.OrderId == Orders[i].OrderId))
                    {
                        Orders.RemoveAt(i);
                    }
                }
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, int newStatusId)
        {
            if (_employeeId == 0)
            {
                StatusMessage = "Ошибка: ID сотрудника не найден.";
                return false;
            }

            var success = await _apiClient.UpdateOrderStatusAsync(orderId, newStatusId, _employeeId);
            if (success)
            {
                StatusMessage = "Статус заказа успешно обновлён!";
                await LoadOrdersAsync();
            }
            else
            {
                StatusMessage = "Ошибка: Не удалось обновить статус заказа.";
            }
            return success;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public enum OrderStatus
        {
            Новый = 1,
            Обрабатывается = 2,
            Отправлен = 3,
            Доставлен = 4,
            Отменён = 5
        }
    }
}
