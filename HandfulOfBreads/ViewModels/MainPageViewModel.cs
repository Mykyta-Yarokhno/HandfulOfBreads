using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandfulOfBreads.Graphics.DrawablePatterns;
using HandfulOfBreads.Services;
using HandfulOfBreads.Services.Interfaces;
using HandfulOfBreads.Views;
using HandfulOfBreads.Views.Popups;
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

        public LocalizationResourceManager LocalizationResourceManager
        {
            get => LocalizationResourceManager.Instance;
        }

        public string StartBeadingButtonText =>
            (string)(IsBeadingActive
                ? LocalizationResourceManager.Instance["StopBeading"]
                : LocalizationResourceManager.Instance["StartBeading"]);

        public ObservableRangeCollection<ColorItemViewModel> AvailableColors { get; } = new();

        public IPatternDrawable CurrentPattern { get; private set; }

        private readonly IPopupService _popupService;
        private readonly GridSavingService _imageSavingService;
        private readonly AppDbContext _context;

        [ObservableProperty]
        private bool isBeadingActive;
        [ObservableProperty]
        private IDrawable drawable;
        [ObservableProperty]
        private Color selectedColor = Colors.White;

        //private int _columns;
        //private int _rows;
        private IImage? _image;

        [ObservableProperty]
        private int _columns;
        [ObservableProperty]
        private int _rows;
        [ObservableProperty]
        private string? _selectedPattern;
        [ObservableProperty]
        private int _drop;

        public MainPageViewModel(
            IPopupService popupService,
            AppDbContext context,
            GridSavingService imageSavingService)
        {
            _popupService = popupService;
            _context = context;
            _imageSavingService = imageSavingService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Columns", out var columnsObject))
            {
                Columns = (int)columnsObject;
            }

            if (query.TryGetValue("Rows", out var rowsObject))
            {
                Rows = (int)rowsObject;
            }

            if (query.TryGetValue("SelectedPattern", out var patternObject))
            {
                SelectedPattern = (string)patternObject;
            }

            List<List<Color>>? grid = null;
            if (query.TryGetValue("Grid", out var gridObject) && gridObject is List<List<Color>> gridList)
            {
                grid = gridList;
            }

            if(query.TryGetValue("Drop",out var dropObject))
            {
                Drop = (int)dropObject;
            }
            CurrentPattern = SelectedPattern switch
            {
                "Loom" => new LoomPatternDrawable(),
                "Brick" => new BrickPatternDrawable(),
                "Peyote" => new PeyotePatternDrawable(),
            };

            var image = LoadImage();
            Drawable = CurrentPattern;

            if (grid != null)
                CurrentPattern.InitializeGrid(Rows, Columns, PixelSize, image, grid, Drop);
            else
                CurrentPattern.InitializeGrid(Rows, Columns, PixelSize, image,drop: Drop);
        }

        public void Initialize(int columns, int rows, string selectedPattern, List<List<Color>>? grid = null , int drop = 1)
        {
            CurrentPattern = selectedPattern switch
            {
                "Loom" => new LoomPatternDrawable(),
                "Brick" => new BrickPatternDrawable(),
                "Peyote" => new PeyotePatternDrawable(),
            };

            var image = LoadImage();
            Drawable = CurrentPattern;

            if (grid != null)
                CurrentPattern.InitializeGrid(rows, columns, PixelSize, image, grid, drop);
            else
                CurrentPattern.InitializeGrid(rows, columns, PixelSize, image, drop:drop);

            _columns = columns;
            _rows = rows;
            _image = image;
            _drop = drop;
        }

        private IImage LoadImage()
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using Stream stream = assembly.GetManifestResourceStream("HandfulOfBreads.Resources.Images.bonk.png");
            return PlatformImage.FromStream(stream);
        }

        public event Action InvalidateRequested;

        [RelayCommand]
        private async Task Clear()
        {
            bool answer = await Application.Current.MainPage.DisplayAlert(
                (string)LocalizationResourceManager.Instance["Confirmation"],
                (string)LocalizationResourceManager.Instance["ConfirmClearMessage"],
                (string)LocalizationResourceManager.Instance["Yes"],
                (string)LocalizationResourceManager.Instance["Cancel"]);

            if (answer)
            {
                CurrentPattern.InitializeGrid(_rows, _columns, PixelSize, _image,drop: _drop);
                InvalidateRequested?.Invoke();
            }
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

        public void ResetSelectedColor()
        {
            SelectedColor = Colors.Transparent;
            CurrentPattern.SelectedColor = Colors.Transparent;
        }

        [RelayCommand]
        private async Task NewDesignAsync()
        {
            //await _popupService.ShowPopupAsync(new NewGraphicsViewPopup(new GridLoadingService()));

            var popup = new NewGraphicsViewPopup();

            popup.ResultReady += OnPopupResultForUpdate;

            _popupService.ShowPopupAsync(popup);
        }

        private void OnPopupResultForUpdate(object sender, NewGraphicsViewPopup.StartPopupResultEventArgs e)
        {
            ((NewGraphicsViewPopup)sender).ResultReady -= OnPopupResultForUpdate;

            CurrentPattern.InitializeGrid(e.Rows, e.Columns, PixelSize, _image, drop: e.Drop);

            Columns = e.Columns;
            Rows = e.Rows;

            InvalidateRequested?.Invoke();
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            await _imageSavingService.SaveImageToGalleryAsync(CurrentPattern);
        }

        

        [RelayCommand]
        public void StartBeading()
        {
            IsBeadingActive = !IsBeadingActive;

            if (IsBeadingActive)
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