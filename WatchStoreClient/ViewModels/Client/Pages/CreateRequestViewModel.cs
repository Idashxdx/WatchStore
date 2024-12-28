using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Client.Pages
{
    public class CreateRequestViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;
        private readonly int _userId;

        private string _description = string.Empty;
        private string _requestType = string.Empty;
        private string? _targetWatchName;
        private string? _targetBrand;
        private decimal? _targetPriceRange;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public string RequestType
        {
            get => _requestType;
            set
            {
                _requestType = value;
                OnPropertyChanged();
            }
        }

        public string? TargetWatchName
        {
            get => _targetWatchName;
            set
            {
                _targetWatchName = value;
                OnPropertyChanged();
            }
        }

        public string? TargetBrand
        {
            get => _targetBrand;
            set
            {
                _targetBrand = value;
                OnPropertyChanged();
            }
        }

        public decimal? TargetPriceRange
        {
            get => _targetPriceRange;
            set
            {
                _targetPriceRange = value;
                OnPropertyChanged();
            }
        }

        private DateTimeOffset? _deadline;

        public DateTimeOffset? Deadline
        {
            get => _deadline;
            set
            {
                _deadline = value;
                OnPropertyChanged();
            }
        }


        public ICommand CreateRequestCommand { get; }

        public CreateRequestViewModel(int userId)
        {
            _userId = userId;
            _apiClient = new ApiClient("http://localhost:5148/");
            CreateRequestCommand = new RelayCommand(async _ => await CreateRequestAsync());
        }

        private async Task CreateRequestAsync()
        {
            var newRequest = new ApiClient.CreateRequestRequest
            {
                UserId = _userId,
                Description = Description,
                RequestType = RequestType,
                TargetWatchName = TargetWatchName,
                TargetBrand = TargetBrand,
                TargetPriceRange = TargetPriceRange
            };

            var success = await _apiClient.CreateRequestAsync(newRequest);

            if (success)
            {
                Console.WriteLine("Заявка успешно создана!");
            }
            else
            {
                Console.WriteLine("Ошибка создания заявки.");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
