using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Admin.Pages
{
    public class AddEmployeeWindowViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;
        private readonly Action _onSuccess;

        public AddEmployeeWindowViewModel(Action onSuccess)
        {
            _apiClient = new ApiClient("http://localhost:5148/");
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            _onSuccess = onSuccess;
        }

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;

        public ICommand SaveCommand { get; }

        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email)
                || string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(Password)
                || string.IsNullOrWhiteSpace(Position))
            {
                Console.WriteLine("Все поля должны быть заполнены.");
                return;
            }

            var request = new ApiClient.AddEmployeeRequest
            {
                Name = Name,
                Email = Email,
                Phone = Phone,
                Password = Password,
                Position = Position
            };

            var isSuccess = await _apiClient.AddEmployeeAsync(request);

            if (isSuccess)
            {
                Console.WriteLine("Сотрудник успешно добавлен!");
                _onSuccess();
            }
            else
            {
                Console.WriteLine("Ошибка при добавлении сотрудника.");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
