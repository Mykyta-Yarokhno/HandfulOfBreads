using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandfulOfBreads.Graphics.DrawablePatterns;
using HandfulOfBreads.Models;
using HandfulOfBreads.Services;
using HandfulOfBreads.Services.Interfaces;
using HandfulOfBreads.Views;
using HandfulOfBreads.Views.Popups;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using MvvmHelpers;
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

        private int _columns;
        private int _rows;
        private IImage? _image;

        public MainPageViewModel()
        {
            _popupService = App.Services.GetRequiredService<IPopupService>();

            _context = App.Services.GetRequiredService<AppDbContext>();
        }

        public void Initialize(int columns, int rows, string selectedPattern, List<List<Color>>? grid = null)
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

            _columns = columns;
            _rows = rows;
            _image = image;
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

        //public string PaletteName = "Preciosa Rocialles";

        //public async Task LoadPaletteAsync(string paletteName)
        //{
        //    var palette = await _context.Palettes
        //        .AsNoTracking()
        //        .Include(p => p.Colors)
        //        .FirstOrDefaultAsync(p => p.Name == paletteName);

        //    if (palette == null)
        //    {
        //        MainThread.BeginInvokeOnMainThread(() => AvailableColors.Clear());
        //        return;
        //    }

        //    PaletteName = paletteName;

        //    var wrapped = palette.Colors.Select(c => new ColorItemViewModel(c));
        //    AvailableColors.ReplaceRange(wrapped);
        //}

        public event Action? RequestInvalidate;

        [RelayCommand]
        private async Task Clear()
        {
            CurrentPattern.InitializeGrid(_rows, _columns, PixelSize, _image);

            RequestInvalidate?.Invoke();
        }

        [RelayCommand]  
        private void SelectColor(ColorItemViewModel colorItem)
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
            Application.Current.MainPage.Navigation.PushAsync(App.Services.GetRequiredService<NewDesignStartPage>());
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
    }
}
