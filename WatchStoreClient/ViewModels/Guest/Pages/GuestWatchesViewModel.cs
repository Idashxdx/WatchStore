using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Guest.Pages
{
    public class GuestWatchesViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;

        private string _filterText = string.Empty;
        private string _selectedSortOption = "По возрастанию цены";

        private ObservableCollection<ApiClient.Watch> _allWatches = new();
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
                    ApplyFilters();
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
                    ApplyFilters();
                }
            }
        }

        public ObservableCollection<string> SortOptions { get; } = new()
        {
            "По возрастанию цены",
            "По убыванию цены"
        };

        public GuestWatchesViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5148/"); 
            LoadWatchesAsync();
        }

        public async void LoadWatchesAsync()
        {
            var watches = await _apiClient.GetAvailableWatchesAsync();
            _allWatches.Clear();

            if (watches != null) 
            {
                foreach (var watch in watches)
                    _allWatches.Add(watch);
            }
            
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = _allWatches.AsEnumerable();

            if (!string.IsNullOrEmpty(FilterText))
            {
                filtered = filtered.Where(watch =>
                    watch.ModelName.Contains(FilterText, System.StringComparison.OrdinalIgnoreCase));
            }

            filtered = SelectedSortOption switch
            {
                "По возрастанию цены" => filtered.OrderBy(watch => watch.Price),
                "По убыванию цены" => filtered.OrderByDescending(watch => watch.Price),
                _ => filtered
            };

            Watches.Clear();

            foreach (var watch in filtered)
                Watches.Add(watch);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
