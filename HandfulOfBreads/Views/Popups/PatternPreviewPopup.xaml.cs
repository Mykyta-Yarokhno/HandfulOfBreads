using CommunityToolkit.Maui.Views;
using HandfulOfBreads.Services;
using System.Text;
using System.Text.Json;

namespace HandfulOfBreads.Views.Popups;

public partial class PatternPreviewPopup : Popup
{
    private readonly string _filePath;
    private readonly ImageLoadingService _imageLoadingService;
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

    public PatternPreviewPopup(string filePath, ImageLoadingService imageLoadingService)
    {
        InitializeComponent();
        _filePath = filePath;

        var screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        PreviewImage.HeightRequest = screenHeight * 0.4;

        PreviewImage.Source = ImageSource.FromFile(filePath);

        _imageLoadingService = imageLoadingService;
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        this.Close();
    }

    private async void OnOpenClicked(object sender, EventArgs e)
    {
        try
        {
            var (name, rows, columns, pixelSize, grid) = await _imageLoadingService.LoadGridFromFileAsync(_filePath);
            this.Close();

            await Application.Current.MainPage.Navigation.PushAsync(new MainPage(columns, rows, name, grid));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Помилка", ex.Message, "OK");
        }
    }
}