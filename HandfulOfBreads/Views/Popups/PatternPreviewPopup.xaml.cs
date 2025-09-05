using CommunityToolkit.Maui.Views;
using HandfulOfBreads.Services;
using System.Text;
using System.Text.Json;

namespace HandfulOfBreads.Views.Popups;

public partial class PatternPreviewPopup : Popup
{
    private readonly string _filePath;
    private readonly GridLoadingService _imageLoadingService;
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

    public PatternPreviewPopup(string filePath, GridLoadingService imageLoadingService)
    {
        InitializeComponent();
        _filePath = filePath;

        var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        PreviewImage.HeightRequest = screenHeight * 0.4;

        PreviewImage.Source = ImageSource.FromFile(filePath);

        _imageLoadingService = imageLoadingService;

        Opened += OnPopupOpened;
    }

    private async void OnPopupOpened(object? sender, EventArgs e)
    {
        if (PopupFrame != null)
        {
            await PopupFrame.TranslateTo(0, 0, 400, Easing.CubicOut);
        }
    }

    private void OnTappedOutside(object? sender, TappedEventArgs e)
    {
        var tapLocation = e.GetPosition(TapHandlerLayout);

        if (tapLocation.HasValue && !PopupFrame.Bounds.Contains(tapLocation.Value.X, tapLocation.Value.Y))
        {
            CloseWithAnimation();
        }
    }

    private async void CloseWithAnimation(object? result = null)
    {
        if (PopupFrame != null)
        {
            await PopupFrame.TranslateTo(0, PopupFrame.Height, 250, Easing.CubicIn);
        }

        Close(result);
    }


    private void OnCancelClicked(object sender, EventArgs e)
    {
        CloseWithAnimation();
    }

    private async void OnOpenClicked(object sender, EventArgs e)
    {
        try
        {
            this.Close();

            var (name, rows, columns, _, grid) = await _imageLoadingService.LoadGridFromFileAsync(_filePath);

            var navigationParameters = new Dictionary<string, object>
        {
            { "Rows", rows },
            { "Columns", columns },
            { "SelectedPattern", name },
            { "Grid", grid }
        };

            await Shell.Current.GoToAsync(nameof(MainPage), navigationParameters);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("???????", ex.Message, "OK");
        }
    }
}