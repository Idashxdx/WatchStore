using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchStoreClient.Services;
using static WatchStoreClient.Services.ApiClient;


namespace WatchStoreClient.ViewModels
{
    public class RegisterWindowViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;
        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _phone = string.Empty;
        private string _statusMessage = string.Empty;

        public RegisterWindowViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5148/");
            RegisterCommand = new RelayCommand(async _ => await RegisterAsync());
        }

        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public string Email
        {
            get => _email;
            set => SetField(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetField(ref _password, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetField(ref _phone, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        public ICommand RegisterCommand { get; }

        private async Task RegisterAsync()
        {
            var registrationData = new RegistrationRequest
            {
                Name = Name,
                Email = Email,
                Password = Password,
                Phone = Phone
            };

            var success = await _apiClient.RegisterAsync(registrationData);
            StatusMessage = success ? "Регистрация прошла успешно!" : "Ошибка при регистрации.";
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
