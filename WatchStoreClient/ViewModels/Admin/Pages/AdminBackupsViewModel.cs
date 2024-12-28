using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Admin.Pages
{
    public class AdminBackupsViewModel : ViewModelBase
    {
        private readonly ApiClient _apiClient;
        private string? _statusMessage;
        private string? _errorMessage;
        private string? _selectedBackup;

        public AdminBackupsViewModel()
        {
            _apiClient = new ApiClient("http://localhost:5148/");

            BackupFiles = new ObservableCollection<string>();
            LoadBackupsCommand = new RelayCommand(async _ => await LoadBackupsAsync());
            CreateBackupCommand = new RelayCommand(async _ => await CreateBackupAsync());
            RestoreBackupCommand = new RelayCommand(async _ => await RestoreBackupAsync());

            LoadBackupsCommand.Execute(null);
        }

        public ObservableCollection<string> BackupFiles { get; set; }

        public ICommand LoadBackupsCommand { get; }
        public ICommand CreateBackupCommand { get; }
        public ICommand RestoreBackupCommand { get; }

        public string? SelectedBackup
        {
            get => _selectedBackup;
            set
            {
                _selectedBackup = value;
                OnPropertyChanged();
            }
        }

        public string? StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        private async Task LoadBackupsAsync()
        {
            try
            {
                ErrorMessage = null;
                StatusMessage = "Загружаем список резервных копий...";
                BackupFiles.Clear();

                var backups = await _apiClient.GetBackupFilesAsync();
                if (backups == null)
                {
                    ErrorMessage = "Ошибка загрузки списка резервных копий.";
                    return;
                }

                foreach (var backup in backups)
                {
                    BackupFiles.Add(backup);
                }
                StatusMessage = "Список резервных копий загружен.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }

        private async Task CreateBackupAsync()
        {
            try
            {
                ErrorMessage = null;
                StatusMessage = "Создаем резервную копию...";

                var success = await _apiClient.CreateBackupAsync();
                if (!success)
                {
                    ErrorMessage = "Не удалось создать резервную копию.";
                    return;
                }

                StatusMessage = "Резервная копия успешно создана.";
                await LoadBackupsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }

        private async Task RestoreBackupAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedBackup))
                {
                    ErrorMessage = "Выберите резервную копию для восстановления.";
                    return;
                }

                ErrorMessage = null;
                StatusMessage = $"Восстанавливаем резервную копию: {SelectedBackup}";

                var success = await _apiClient.RestoreBackupAsync(SelectedBackup);
                if (!success)
                {
                    ErrorMessage = "Не удалось выполнить восстановление.";
                    return;
                }

                StatusMessage = "Восстановление выполнено успешно.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }
    }
}
