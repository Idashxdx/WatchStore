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
    public class ClientRequestsViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;
        private readonly int _userId;

        public ObservableCollection<ApiClient.UserRequest> Requests { get; } = new();

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

        public ICommand ReloadRequestsCommand { get; }

        public ClientRequestsViewModel(int userId)
        {
            _userId = userId;
            _apiClient = new ApiClient("http://localhost:5148/");

            ReloadRequestsCommand = new RelayCommand(async _ => await LoadRequestsAsync());
            LoadRequestsAsync();
        }

        public async Task LoadRequestsAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                var requests = await _apiClient.GetUserRequestHistoryAsync(_userId);

                Requests.Clear();
                if (requests != null)
                {
                    foreach (var request in requests)
                    {
                        Requests.Add(request);
                    }
                }
                else
                {
                    ErrorMessage = "Не удалось загрузить историю заявок.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при загрузке заявок: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
