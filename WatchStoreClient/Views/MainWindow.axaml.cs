using System;
using Avalonia.Controls;
using WatchStoreClient.ViewModels;
using WatchStoreClient.Views.Admin;
using WatchStoreClient.Views.Client;
using WatchStoreClient.Views.Employee;
using WatchStoreClient.Views.Guest;


namespace WatchStoreClient.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(OnLoginSuccessful, OnGuestModeSelected);
        }

        private void OpenWindow(Window newWindow)
        {
            newWindow.Show();
            this.Close(); 
        }

        // Обрабатываем успешный логин
        private void OnLoginSuccessful(string role, int? userId)
        {
            switch (role)
            {
                case "Admin":
                    OpenWindow(new AdminWindow());
                    break;
                case "Employee":
                    OpenWindow(new EmployeeWindow(userId ?? 0));
                    break;
                case "Client":
                    OpenWindow(new ClientWindow(userId ?? 0));
                    break;
            }
        }

        private void OnGuestModeSelected()
        {
            var guestWindow = new GuestWindow(() =>
            {
                new MainWindow().Show();
            });
            guestWindow.Show();
            this.Close();
        }



    }
}
