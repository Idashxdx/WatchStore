using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Client.Pages
{
    public class ClientWatchesViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;

        private readonly int _userId;

        public int UserId => _userId;

        private string _filterText = string.Empty;
        private string _selectedSortOption = "По возрастанию цены";

        public ObservableCollection<ApiClient.Watch> Watches { get; } = new(); 

        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText != value)
                {
                    _filterText = value;
                    OnPropertyChanged();
                    _ = LoadWatchesAsync();
                }
            }
        }

        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (_selectedSortOption != value)
                {
                    _selectedSortOption = value;
                    OnPropertyChanged();
                    _ = LoadWatchesAsync();
                }
            }
        }
        public ObservableCollection<string> SortOptions { get; } = new()
        {
            "По возрастанию цены",
            "По убыванию цены"
        };

        public ICommand ReloadWatchesCommand { get; }

        public ClientWatchesViewModel(int userId)
        {
            _userId = userId;
          
            _apiClient = new ApiClient("http://localhost:5148/");
           
            ReloadWatchesCommand = new RelayCommand(async _ => await ReloadWatches());

            _ = LoadWatchesAsync();
        }

        public async Task LoadWatchesAsync()
        {
            try
            {
                var sortOption = SelectedSortOption switch
                {
                    "По возрастанию цены" => "asc",
                    "По убыванию цены" => "desc",
                    _ => "asc"
                };

                var watches = await _apiClient.GetAvailableWatchesAsync(FilterText, sortOption);

                Watches.Clear();
                if (watches != null)
                {
                    foreach (var watch in watches)
                    {
                        Watches.Add(watch);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке часов: {ex.Message}");
            }
        }
        public async Task ReloadWatches()
        {
            await LoadWatchesAsync();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
