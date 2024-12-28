using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Employee.Pages
{
    public class ArchivedOrdersViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;
        private ObservableCollection<ApiClient.ArchivedOrderView> _orders;

        public ObservableCollection<ApiClient.ArchivedOrderView> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged();
            }
        }

        private bool _sortDescending;
        public bool SortDescending
        {
            get => _sortDescending;
            set
            {
                _sortDescending = value;
                OnPropertyChanged();
                LoadOrders();
            }
        }

        public ArchivedOrdersViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5148/");
            Orders = new ObservableCollection<ApiClient.ArchivedOrderView>();
            LoadOrders();
        }

        private async void LoadOrders()
        {
            try
            {
                Console.WriteLine("Загрузка архивных заказов...");
                var orders = await _apiClient.GetArchivedOrdersAsync(null, null, null, SortDescending);
                if (orders != null)
                {
                    Console.WriteLine($"Загружено {orders.Count} заказов");
                    Orders = new ObservableCollection<ApiClient.ArchivedOrderView>(orders);
                }
                else
                {
                    Console.WriteLine("Не удалось получить заказы.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке заказов: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
