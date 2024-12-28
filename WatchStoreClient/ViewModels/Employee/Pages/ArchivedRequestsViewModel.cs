using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Employee.Pages
{
    public class ArchivedRequestsViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;
        private ObservableCollection<ApiClient.ArchivedRequestView> _requests;

        public ObservableCollection<ApiClient.ArchivedRequestView> Requests
        {
            get => _requests;
            set
            {
                _requests = value;
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
                LoadRequests(); 
            }
        }

        public ArchivedRequestsViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5148/");
            Requests = new ObservableCollection<ApiClient.ArchivedRequestView>();
            LoadRequests();
        }

        private async void LoadRequests()
        {
            try
            {
                Console.WriteLine("Загрузка архивных заявок...");
                var requests = await _apiClient.GetArchivedRequestsAsync(null, null, null, SortDescending);
                if (requests != null)
                {
                    Console.WriteLine($"Загружено {requests.Count} заявок");
                    Requests = new ObservableCollection<ApiClient.ArchivedRequestView>(requests);
                }
                else
                {
                    Console.WriteLine("Не удалось получить заявки.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке заявок: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
