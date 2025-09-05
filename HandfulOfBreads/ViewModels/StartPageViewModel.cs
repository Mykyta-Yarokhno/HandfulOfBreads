using HandfulOfBreads.Resources.Localization;
using HandfulOfBreads.Services;
using HandfulOfBreads.Services.Interfaces;
using HandfulOfBreads.Views.Popups;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HandfulOfBreads.Models;

namespace HandfulOfBreads.ViewModels;

public class StartPageViewModel : BaseViewModel
{
    public LocalizationResourceManager LocalizationResourceManager
    {
        get => LocalizationResourceManager.Instance;
    }

    private readonly IPopupService _popupService;
    private readonly GridLoadingService _imageLoadingService;
    private readonly ImagesLoadingService _imagesLoadingService;

    public ObservableCollection<PixelGridItem> PixelGrids { get; } = new();

    private bool _isRefreshing;
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public ICommand AddNewCommand { get; }
    public ICommand LanguageSwitchCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand ItemTappedCommand { get; }

    public StartPageViewModel(IPopupService popupService, GridLoadingService imageLoadingService, ImagesLoadingService imagesLoadingService)
    {
        _popupService = popupService;
        _imageLoadingService = imageLoadingService;
        _imagesLoadingService = imagesLoadingService; // Assign ImagesLoadingService

        AddNewCommand = new AsyncRelayCommand(ChooseNewAsync);
        LanguageSwitchCommand = new AsyncRelayCommand(OnLanguageSwitch);
        RefreshCommand = new AsyncRelayCommand(LoadPixelGridsAsync);
        ItemTappedCommand = new AsyncRelayCommand<PixelGridItem>(OnItemTappedAsync);

        _ = LoadPixelGridsAsync();
    }

    private async Task ChooseNewAsync()
    {
        var popup = App.Services.GetRequiredService<NewPatternPopup>();

        var result = await _popupService.ShowPopupAsync<string>(popup);

        if (result == "ConvertPhoto")
        {
            var images = await _imagesLoadingService.GetRecentImagesAsync();
            await _popupService.ShowPopupAsync(new ChoosePhotoPopup(images));
        }
    }

    private async Task OnLanguageSwitch()
    {
        var switchToCulture = AppResources.Culture.TwoLetterISOLanguageName
             .Equals("uk", StringComparison.InvariantCultureIgnoreCase) ?
             new CultureInfo("en-US") : new CultureInfo("uk-UA");

        LocalizationResourceManager.Instance.SetCulture(switchToCulture);

        await Task.CompletedTask;
    }

    private async Task LoadPixelGridsAsync()
    {
        IsRefreshing = true;

        try
        {
            var status = await Permissions.RequestAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
            {
                return;
            }

#if ANDROID
            var picturesPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
            var pixelGridFiles = Directory.GetFiles(picturesPath)
                                         .Where(file => Path.GetFileName(file).StartsWith("pixel_grid_"))
                                         .OrderByDescending(file => File.GetLastWriteTime(file))
                                         .ToList();

            PixelGrids.Clear();
            foreach (var file in pixelGridFiles)
            {
                PixelGrids.Add(new PixelGridItem { FilePath = file, ImageSource = ImageSource.FromFile(file) });
            }
#endif
        }
        catch (Exception ex)
        {
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async Task OnItemTappedAsync(PixelGridItem item)
    {
        if (item == null) return;

        try
        {
            var popup = new PatternPreviewPopup(item.FilePath, _imageLoadingService);
            await _popupService.ShowPopupAsync(popup);
        }
        catch (Exception ex)
        {
        }
    }
}

public class PixelGridItem
{
    public string FilePath { get; set; }
    public ImageSource ImageSource { get; set; }
}