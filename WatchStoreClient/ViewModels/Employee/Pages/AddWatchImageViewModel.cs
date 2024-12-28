using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WatchStoreClient.Services;

namespace WatchStoreClient.ViewModels.Employee.Pages
{
    public class AddWatchImageViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient = new("http://localhost:5148/");
        private readonly int _watchId;

        private Bitmap? _imagePreview;
        private byte[]? _imageBytes;   
        private string _errorMessage = string.Empty; 

        public AddWatchImageViewModel(int watchId)
        {
            _watchId = watchId;
            UploadImageCommand = new RelayCommand(async _ => await UploadImageAsync());
            SelectImageCommand = new RelayCommand(async _ => await SelectImageAsync());
        }

        public ICommand UploadImageCommand { get; }  
        public ICommand SelectImageCommand { get; } 

        public Bitmap? ImagePreview
        {
            get => _imagePreview;
            set => SetField(ref _imagePreview, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        private async Task SelectImageAsync()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Выберите изображение для товара",
                    AllowMultiple = false,
                    Filters =
                {
                    new FileDialogFilter { Name = "Изображения", Extensions = { "jpg", "jpeg", "png" } }
                }
                };

                var result = await openFileDialog.ShowAsync(new Window());

                if (result == null || result.Length == 0) return;

                var selectedFilePath = result[0];
                _imageBytes = await File.ReadAllBytesAsync(selectedFilePath);

                using (var stream = File.OpenRead(selectedFilePath))
                {
                    ImagePreview = new Bitmap(stream);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка выбора изображения: {ex.Message}";
            }
        }
        private async Task UploadImageAsync()
        {
            if (_imageBytes == null)
            {
                ErrorMessage = "Вы должны сначала выбрать изображение.";
                return;
            }

            try
            {
                var result = await _apiClient.UploadWatchImageAsync(_watchId, _imageBytes, "watch_image.jpg");
                if (result)
                {
                    ErrorMessage = string.Empty;
                    Console.WriteLine("⚡️ Изображение загружено успешно!");
                }
                else
                {
                    ErrorMessage = "Ошибка загрузки изображения.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (!Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
