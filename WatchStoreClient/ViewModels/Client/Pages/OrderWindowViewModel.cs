using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Client.Pages
{
    public class OrderWindowViewModel : INotifyPropertyChanged
    {
        private readonly Action _onOrderSuccess;

        public ApiClient.Watch SelectedWatch { get; }
        public int UserId { get; }

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value <= 0) _quantity = 1;
                else _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        public string DeliveryAddress { get; set; } = string.Empty;

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        public ICommand ConfirmOrderCommand { get; }

        public OrderWindowViewModel(ApiClient.Watch watch, int userId, Action onOrderSuccess)
        {
            SelectedWatch = watch ?? throw new ArgumentNullException(nameof(watch));
            UserId = userId;
            _onOrderSuccess = onOrderSuccess ?? throw new ArgumentNullException(nameof(onOrderSuccess));

            ConfirmOrderCommand = new RelayCommand(async _ => await SubmitOrderAsync());
        }

        private async Task SubmitOrderAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DeliveryAddress))
                {
                    StatusMessage = "Введите адрес доставки!";
                    return;
                }

                if (Quantity <= 0)
                {
                    StatusMessage = "Введите корректное количество!";
                    return;
                }

                var apiClient = new ApiClient("http://localhost:5148/");
                var isSuccess = await apiClient.CreateOrderAsync(new ApiClient.CreateOrderRequest
                {
                    UserId = UserId,
                    WatchId = SelectedWatch.WatchId,
                    Quantity = Quantity,
                    DeliveryAddress = DeliveryAddress
                });

                if (isSuccess)
                {
                    StatusMessage = "Заказ успешно оформлен!";
                    _onOrderSuccess?.Invoke();
                }
                else
                {
                    StatusMessage = "Произошла ошибка оформления. Попробуйте снова.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
