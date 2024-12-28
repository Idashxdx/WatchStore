using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Employee.Pages
{
    public class EmployeeClientsViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;
        private string _searchName = string.Empty;
        private ObservableCollection<ApiClient.ClientUser> _clients = new ObservableCollection<ApiClient.ClientUser>();

        public ObservableCollection<ApiClient.ClientUser> Clients
        {
            get => _clients;
            private set
            {
                _clients = value;
                OnPropertyChanged();
            }
        }

        public string SearchName
        {
            get => _searchName;
            set
            {
                if (_searchName != value)
                {
                    _searchName = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }

        public EmployeeClientsViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5148/");

            SearchCommand = new RelayCommand(async _ => await LoadClients(SearchName));
            RefreshCommand = new RelayCommand(async _ => await LoadClients(string.Empty));

            Task.Run(() => LoadClients(string.Empty));
        }

        private async Task LoadClients(string name)
        {
            var clients = string.IsNullOrWhiteSpace(name)
                ? await _apiClient.GetAllClientsAsync()
                : await _apiClient.SearchClientsAsync(name);

            if (clients != null)
            {
                Clients.Clear();
                foreach (var client in clients)
                {
                    Clients.Add(client);
                }
            }
        }



        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
