using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WatchStoreClient.ViewModels.Client.Pages;

namespace WatchStoreClient.ViewModels.Client
{
    public class ClientWindowViewModel : INotifyPropertyChanged
    {
        private readonly int _userId;
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowWatchesCommand { get; }
        public ICommand ShowRequestsCommand { get; }
        public ICommand ShowCreateRequestCommand { get; }
        public ICommand ShowOrdersCommand { get; }
        public ICommand LogoutCommand { get; }

        public ClientWindowViewModel(int userId, Action onLogoutCallback)
        {
            _userId = userId;

            CurrentView = new ClientWatchesViewModel(_userId);

            ShowWatchesCommand = new RelayCommand(_ => CurrentView = new ClientWatchesViewModel(_userId));
            ShowRequestsCommand = new RelayCommand(_ => CurrentView = new ClientRequestsViewModel(_userId));
            ShowCreateRequestCommand = new RelayCommand(_ => CurrentView = new CreateRequestViewModel(_userId));
            ShowOrdersCommand = new RelayCommand(_ => CurrentView = new ClientOrdersViewModel(_userId));

            LogoutCommand = new RelayCommand(_ => onLogoutCallback());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
