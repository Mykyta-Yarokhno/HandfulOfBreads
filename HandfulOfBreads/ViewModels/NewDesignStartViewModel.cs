using HandfulOfBreads.Resources.Localization;
using HandfulOfBreads.Services;
using HandfulOfBreads.Views;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HandfulOfBreads.ViewModels
{
    public class NewDesignStartViewModel : BaseViewModel
    {
        private string _columns;
        private string _rows;
        private string _selectedPattern;

        private readonly GridLoadingService _imageLoadingService;

        public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

        public ObservableCollection<string> Patterns { get; } = new() { "Loom", "Brick", "Payote" };

        public string Columns
        {
            get => _columns;
            set
            {
                if (SetProperty(ref _columns, value))
                    ValidateInput();
            }
        }

        public string Rows
        {
            get => _rows;
            set
            {
                if (SetProperty(ref _rows, value))
                    ValidateInput();
            }
        }

        public string SelectedPattern
        {
            get => _selectedPattern;
            set
            {
                if (value == "Payote" || value == "Brick")
                {
                    Application.Current.MainPage.DisplayAlert("Unavailable", "This option is currently disabled.", "OK");
                    SelectedPattern = "Loom";
                }
                else
                {
                    SetProperty(ref _selectedPattern, value);
                }
            }
        }

        private bool _isFormValid;
        public bool IsFormValid
        {
            get => _isFormValid;
            set => SetProperty(ref _isFormValid, value);
        }

        public ICommand OpenExistingCommand { get; }
        public ICommand OkCommand { get; }
        
        public NewDesignStartViewModel(GridLoadingService imageLoadingService)
        {
            _selectedPattern = Patterns[0];

            _rows = "20";
            _columns = "10";
            
            OpenExistingCommand = new Command(async () => await OnOpenExisting());
            OkCommand = new Command(async () => await OnOk(), () => IsFormValid);

            //LanguageSwitchCommand = new Command(OnLanguageSwitch);
            _imageLoadingService = imageLoadingService;

            ValidateInput();
        }

        private void ValidateInput()
        {
            bool isValid = int.TryParse(Columns, out int c) && int.TryParse(Rows, out int r)
                && c >= 0 && c <= 200 && r >= 0 && r <= 200;
            IsFormValid = isValid;
            ((Command)OkCommand).ChangeCanExecute();
        }

        private async Task OnOk()
        {
            int columns = int.Parse(Columns);
            int rows = int.Parse(Rows);

                    var navigationParameters = new Dictionary<string, object>
            {
                { "Columns", columns },
                { "Rows", rows },
                { "SelectedPattern", SelectedPattern }
            };

            // Navigate using the Shell and pass the parameters.
            await Shell.Current.GoToAsync(nameof(MainPage), navigationParameters);
        }

        private async Task OnOpenExisting()
        {
            try
            {
                var fileResult = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Оберіть ваш збережений малюнок",
                    FileTypes = FilePickerFileType.Images
                });

                if (fileResult == null) return;
                var filePath = fileResult.FullPath;

                if (!Path.GetFileName(filePath).StartsWith("pixel_grid_"))
                {
                    await Application.Current.MainPage.DisplayAlert("Помилка", "Цей файл не є малюнком програми.", "OK");
                    return;
                }

                var (name, rows, columns, pixelSize, grid) = await _imageLoadingService.LoadGridFromFileAsync(filePath);

                var navigationParameters = new Dictionary<string, object>
                {
                    { "Columns", columns },
                    { "Rows", rows },
                    { "SelectedPattern", name },
                    { "Grid", grid }
                };

                // Navigate using the Shell and pass the parameters.
                await Shell.Current.GoToAsync(nameof(MainPage), navigationParameters);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
            }
        }
    }
}
