using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandfulOfBreads.Graphics.DrawablePatterns;
using HandfulOfBreads.Models;
using HandfulOfBreads.Services;
using HandfulOfBreads.Services.Interfaces;
using HandfulOfBreads.Views.Popups;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using MvvmHelpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Colors = Microsoft.Maui.Graphics.Colors;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace HandfulOfBreads.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        private const int PixelSize = 40;
        private readonly GridSavingService _imageSavingService = new();

        public LocalizationResourceManager LocalizationResourceManager
        => LocalizationResourceManager.Instance;

        public string StartBeadingButtonText =>
            (string)(IsBeadingActive
                ? LocalizationResourceManager.Instance["StopBeading"]
                : LocalizationResourceManager.Instance["StartBeading"]);

        //public ObservableCollection<ColorItem> AvailableColors { get; } = new();

        public ObservableRangeCollection<ColorItemViewModel> AvailableColors { get; } = new();

        public IPatternDrawable CurrentPattern { get; private set; }

        private readonly IPopupService _popupService;

        private readonly AppDbContext _context;

        [ObservableProperty] 
        private bool isBeadingActive;
        [ObservableProperty]
        private IDrawable drawable;
        [ObservableProperty]
        private Color selectedColor = Colors.White;

        public MainPageViewModel(int columns, int rows, string selectedPattern, List<List<Color>>? grid = null)
        {
            if (selectedPattern == "Loom")
                CurrentPattern = new LoomPatternDrawable();
            else if (selectedPattern == "Brick")
                CurrentPattern = new BrickPatternDrawable();
            else
                CurrentPattern = new LoomPatternDrawable();

            var image = LoadImage();

            Drawable = CurrentPattern;

            if (grid != null)
                CurrentPattern.InitializeGrid(rows, columns, PixelSize, image, grid);
            else
                CurrentPattern.InitializeGrid(rows, columns, PixelSize, image);

            _popupService = App.Services.GetRequiredService<IPopupService>();

            _context = App.Services.GetRequiredService<AppDbContext>();
        }

        private IImage LoadImage()
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using Stream stream = assembly.GetManifestResourceStream("HandfulOfBreads.Resources.Images.bonk.png");
            return PlatformImage.FromStream(stream);

            //IImage image;

            //Assembly assembly = GetType().GetTypeInfo().Assembly;

            //using (Stream stream = assembly.GetManifestResourceStream("HandfulOfBreads.Resources.Images.bonk.png"))
            //{

            //    image = PlatformImage.FromStream(stream);
            //}

            //return image;
        }

        public async Task LoadPaletteAsync(string paletteName)
        {
            var palette = await _context.Palettes
                .AsNoTracking()
                .Include(p => p.Colors)
                .FirstOrDefaultAsync(p => p.Name == paletteName);

            if (palette == null)
            {
                MainThread.BeginInvokeOnMainThread(() => AvailableColors.Clear());
                return;
            }

            var wrapped = palette.Colors.Select(c => new ColorItemViewModel(c));
            AvailableColors.ReplaceRange(wrapped);
        }

        [RelayCommand]
        private void SelectColor(ColorItem colorItem)
        {
            if (colorItem is null)
                return;

            var color = Color.FromArgb(colorItem.HexColor);
            SelectedColor = color;
            CurrentPattern.SelectedColor = color;
        }

        [RelayCommand]
        private async Task NewDesignAsync()
        {
            //var result = new NewGraphicsViewPopup();
            //await this.ShowPopupAsync(result);

            //InitializeDrawable(result.FirstNumber, result.SecondNumber);
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            await _imageSavingService.SaveImageToGalleryAsync(CurrentPattern);
        }

        public event Action InvalidateRequested;

        [RelayCommand]
        public void StartBeading()
        {
            IsBeadingActive = !IsBeadingActive;

            if(IsBeadingActive)
                HighlightRow(0);
            else
                HighlightRow(null);
        }

        partial void OnIsBeadingActiveChanged(bool oldValue, bool newValue)
        {
            OnPropertyChanged(nameof(StartBeadingButtonText));
        }

        [RelayCommand]
        public void MoveRowUp() => HighlightRow(-1);

        [RelayCommand]
        public void MoveRowDown() => HighlightRow(1);

        private void HighlightRow(int? direction)
        {
            CurrentPattern.HighlightRow(direction);
            InvalidateRequested?.Invoke();
        }

        [RelayCommand]
        private async Task OpenColorPicker()
        {

            var popup = App.Services.GetRequiredService<ColorPickerPopup>();

            var result = await _popupService.ShowPopupAsync<Color>(popup);

            if (result is Color selectedColor)
            {
                CurrentPattern.SelectedColor = selectedColor;
                SelectedColor = selectedColor;
            }
        }

    }
}
