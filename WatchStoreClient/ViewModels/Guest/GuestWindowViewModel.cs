using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WatchStoreClient.ViewModels.Guest.Pages;
using System.Windows.Input;

namespace WatchStoreClient.ViewModels.Guest
{
    public class GuestWindowViewModel : INotifyPropertyChanged
    {
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
        public ICommand ShowRequestCommand { get; }
        public ICommand LogoutCommand { get; }

        // Конструктор
        public GuestWindowViewModel(Action onLogoutCallback)
        {
            CurrentView = new GuestWatchesViewModel();

            ShowWatchesCommand = new RelayCommand(_ => CurrentView = new GuestWatchesViewModel());
            ShowRequestCommand = new RelayCommand(_ => CurrentView = new GuestRequestViewModel());
            LogoutCommand = new RelayCommand(_ => onLogoutCallback());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

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
