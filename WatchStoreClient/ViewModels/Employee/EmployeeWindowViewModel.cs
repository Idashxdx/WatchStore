using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WatchStoreClient.ViewModels.Employee.Pages;
using WatchStoreClient.Views.Employee.Pages;

namespace WatchStoreClient.ViewModels.Employee
{
    public class EmployeeWindowViewModel : INotifyPropertyChanged
    {

        private readonly int _employeeId;
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
        public ICommand ShowOrdersCommand { get; }
        public ICommand ShowClientsCommand { get; }
        public ICommand ShowArchivedOrdersCommand { get; }
        public ICommand ShowArchivedRequestsCommand { get; }
        public ICommand LogoutCommand { get; }

        public EmployeeWindowViewModel(int employeeId, Action onLogoutCallback)
        {
            _employeeId = employeeId;
           
            CurrentView = new EmployeeWatchesViewModel();
            ShowWatchesCommand = new RelayCommand(_ => CurrentView = new EmployeeWatchesViewModel());
            ShowRequestsCommand = new RelayCommand(_ => CurrentView = new EmployeeRequestsView(new EmployeeRequestsViewModel(employeeId)));
            ShowOrdersCommand = new RelayCommand(_ => CurrentView = new EmployeeOrdersViewModel(_employeeId));
            ShowClientsCommand = new RelayCommand(_ => CurrentView = new EmployeeClientsViewModel());
            ShowArchivedRequestsCommand = new RelayCommand(_ => CurrentView = new ArchivedRequestsViewModel());
            ShowArchivedOrdersCommand = new RelayCommand(_ => CurrentView = new ArchivedOrdersViewModel());

            LogoutCommand = new RelayCommand(_ => onLogoutCallback());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
