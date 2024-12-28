using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchStoreClient.Services;
using WatchStoreClient.Views;

namespace WatchStoreClient.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private int? _userId;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _loginStatus = string.Empty;

        private readonly ApiClient _apiClient;
        private readonly Action<string, int?> _onLoginSuccessful; 
        private readonly Action _onGuestModeSelected;

        public int? UserId
        {
            get => _userId;
            private set => SetField(ref _userId, value);
        }
        // Конструктор
        public MainWindowViewModel(Action<string, int?> onLoginSuccessful, Action onGuestModeSelected)
        {
            _apiClient = new ApiClient("http://localhost:5148/"); 
            _onLoginSuccessful = onLoginSuccessful; 
            _onGuestModeSelected = onGuestModeSelected; 

            LoginCommand = new RelayCommand(async _ => await LoginAsync());
            GuestModeCommand = new RelayCommand(_ => EnterGuestMode());
            OpenRegisterWindowCommand = new RelayCommand(_ => OpenRegisterWindow());
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

        public string LoginStatus
        {
            get => _loginStatus;
            set => SetField(ref _loginStatus, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand GuestModeCommand { get; }
        public ICommand OpenRegisterWindowCommand { get; }

        private async Task LoginAsync()
        {
            try
            {
                var response = await _apiClient.LoginAsync(Email, Password);

                if (response != null)
                {
                    Console.WriteLine($"[DEBUG] Авторизация прошла успешно для пользователя с ID: {response.UserId}");
                    UserId = response.UserId; // Сохраняем ID пользователя.
                    _onLoginSuccessful(response.Role, response.UserId); // Передаём ID в callback.
                    LoginStatus = string.Empty;
                }
                else
                {
                    LoginStatus = "Неверные email или пароль.";
                }
            }
            catch (Exception ex)
            {
                LoginStatus = $"Ошибка подключения: {ex.Message}";
            }
        }



        // Метод для входа в гостевой режим
        private void EnterGuestMode()
        {
            _onGuestModeSelected();
        }

        // Открытие окна регистрации
        private void OpenRegisterWindow()
        {
            var registerWindow = new RegisterWindow(); // Создаём экземпляр окна регистрации
            registerWindow.Show();                    // Открываем окно
        }

        // Вспомогательный метод установки значений свойств (для биндинга)
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

        // Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Реализация команды (RelayCommand) для обработки действий UI
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => _execute(parameter);
    }
}
