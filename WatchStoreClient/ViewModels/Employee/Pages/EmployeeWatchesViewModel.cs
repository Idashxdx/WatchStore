using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Employee.Pages
{
    public class EmployeeWatchesViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient = new("http://localhost:5148/");
        private string _filterText = string.Empty;
        private ObservableCollection<ApiClient.Watch> _allWatches = new();
        public ObservableCollection<ApiClient.Watch> Watches { get; } = new();

        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                OnPropertyChanged();
                ApplyFilter();
            }
        }

        public ICommand AddWatchCommand { get; }

        public EmployeeWatchesViewModel()
        {
            LoadWatches();
        }

        private async void LoadWatches()
        {
            var watches = await _apiClient.GetAvailableWatchesAsync();
            UpdateWatchLists(watches);
        }

        public async Task LoadWatchesAsync()
        {
            var watches = await _apiClient.GetAvailableWatchesAsync();
            UpdateWatchLists(watches);
        }

        private void UpdateWatchLists(IEnumerable<ApiClient.Watch>? watches)
        {
            // Очищаем оба списка
            _allWatches.Clear();
            Watches.Clear();

            if (watches != null)
            {
                foreach (var watch in watches)
                {
                    _allWatches.Add(watch); 
                    Watches.Add(watch);
                }
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(_filterText))
            {
                Watches.Clear();
                foreach (var watch in _allWatches)
                {
                    Watches.Add(watch);
                }
            }
            else
            {
                var filtered = _allWatches.Where(w => 
                    w.ModelName.Contains(_filterText, StringComparison.OrdinalIgnoreCase));

                Watches.Clear();
                foreach (var watch in filtered)
                {
                    Watches.Add(watch);
                }
            }
        }

        public async Task<bool> RemoveWatchAsync(int watchId)
        {
            var result = await _apiClient.RemoveWatchAsync(watchId);
            if (result)
            {
                var watchToRemove = _allWatches.FirstOrDefault(w => w.WatchId == watchId);
                if (watchToRemove != null)
                {
                    _allWatches.Remove(watchToRemove);
                    Watches.Remove(watchToRemove);
                }
            }
            return result;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
