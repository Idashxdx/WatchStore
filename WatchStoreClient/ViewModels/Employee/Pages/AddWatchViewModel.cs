using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Employee.Pages
{
    public class AddWatchViewModel : INotifyPropertyChanged
    {

        private readonly Action _onCloseCallback;
        private readonly ApiClient _apiClient;

        private string _name = string.Empty;
        private string _brand = string.Empty;
        private int _typeId;
        private decimal _price;
        private int _quantity;
        private string? _caseMaterial;
        private string? _strapMaterial;
        private decimal? _caseDiameter;
        private string? _waterResistance;

        private string _errorMessage = string.Empty;

        public AddWatchViewModel(Action onCloseCallback)
        {
            _onCloseCallback = onCloseCallback;
            _apiClient = new ApiClient("http://localhost:5148/");

            AddWatchCommand = new RelayCommand(async _ => await AddWatchAsync());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }
        public string Brand
        {
            get => _brand;
            set => SetField(ref _brand, value);
        }
        public int TypeId
        {
            get => _typeId;
            set => SetField(ref _typeId, value);
        }
        public decimal Price
        {
            get => _price;
            set => SetField(ref _price, value);
        }
        public int Quantity
        {
            get => _quantity;
            set => SetField(ref _quantity, value);
        }
        public string? CaseMaterial
        {
            get => _caseMaterial;
            set => SetField(ref _caseMaterial, value);
        }
        public string? StrapMaterial
        {
            get => _strapMaterial;
            set => SetField(ref _strapMaterial, value);
        }
        public decimal? CaseDiameter
        {
            get => _caseDiameter;
            set => SetField(ref _caseDiameter, value);
        }
        public string? WaterResistance
        {
            get => _waterResistance;
            set => SetField(ref _waterResistance, value);
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        // Команды
        public ICommand AddWatchCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task AddWatchAsync()
        {
            try
            {
                ErrorMessage = string.Empty;

                var request = new ApiClient.AddWatchRequest
                {
                    Name = Name,
                    Brand = Brand,
                    TypeId = TypeId,
                    Price = Price,
                    Quantity = Quantity,
                    CaseMaterial = CaseMaterial,
                    StrapMaterial = StrapMaterial,
                    CaseDiameter = CaseDiameter,
                    WaterResistance = WaterResistance
                };

                var result = await _apiClient.AddWatchAsync(request);

                if (result)
                {
                    _onCloseCallback();
                }
                else
                {
                    ErrorMessage = "Ошибка при добавлении товара. Проверьте данные и попробуйте ещё раз.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Произошла ошибка: {ex.Message}";
            }
        }

        private void Cancel()
        {
            _onCloseCallback();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (!Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }
        
    }
}
