using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Client.Pages
{
    public class ClientOrdersViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;
        private readonly int _userId;

        public ObservableCollection<ApiClient.UserOrder> Orders { get; } = new();

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand ReloadOrdersCommand { get; }

        public ClientOrdersViewModel(int userId)
        {
            _userId = userId;
            _apiClient = new ApiClient("http://localhost:5148/");

            ReloadOrdersCommand = new RelayCommand(async _ => await LoadOrdersAsync());
            LoadOrdersAsync();
        }

        public async Task LoadOrdersAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                var orders = await _apiClient.GetUserOrderHistoryAsync(_userId);

                Orders.Clear();
                if (orders != null)
                {
                    foreach (var order in orders)
                    {
                        Orders.Add(order);
                    }
                }
                else
                {
                    ErrorMessage = "Не удалось загрузить историю заказов.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке данных: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
