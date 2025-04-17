using CommunityToolkit.Maui.Views;
using HandfulOfBreads.Services;
using HandfulOfBreads.ViewModels;
using HandfulOfBreads.Views.Popups;
using System.Text;
using System.Text.Json;

namespace HandfulOfBreads.Views;

public partial class StartPage : ContentPage
{
    private readonly GridLoadingService _imageLoadingService;
    public StartPage(StartPageViewModel viewModel, GridLoadingService imageLoadingService)
    {
        InitializeComponent();
        BindingContext = viewModel;
        Shell.SetNavBarIsVisible(this, false);
        _imageLoadingService = imageLoadingService;
        LoadPixelGrids();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await Task.Delay(500);
        LoadPixelGrids();
        refreshView.IsRefreshing = false;
    }

    private async void LoadPixelGrids()
    {
        var status = await Permissions.RequestAsync<Permissions.StorageRead>();

        try
        {
#if ANDROID
            var picturesPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
            var pixelGridFiles = Directory.GetFiles(picturesPath)
                                           .Where(file => Path.GetFileName(file).StartsWith("pixel_grid_"))
                                           .OrderByDescending(file => File.GetLastWriteTime(file))
                                           .ToList();

            stackLayout.Children.Clear();

            var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
            double itemSize = screenWidth switch
            {
                <= 500 => 150,    
                <= 800 => 220,     
                _ => 220        
            };

            foreach (var file in pixelGridFiles)
            {
                var image = new Image
                {
                    Source = ImageSource.FromFile(file),
                    Aspect = Aspect.AspectFit,
                    WidthRequest = itemSize,
                    HeightRequest = itemSize
                };

                var frame = new Frame
                {
                    Content = image,
                    Margin = new Thickness(10),
                    WidthRequest = itemSize+4,
                    HeightRequest = itemSize+4,
                    CornerRadius = 0,
                    HasShadow = false,
                    BorderColor = Color.FromArgb("#553d3a"),
                    BackgroundColor = Colors.Wheat
                };

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += async (s, e) =>
                {
                    var popup = new PatternPreviewPopup(file , _imageLoadingService);
                    await Application.Current.MainPage.ShowPopupAsync(popup);
                };

                frame.GestureRecognizers.Add(tapGesture);

                stackLayout.Children.Add(frame);
            }
#else
            Application.Current.MainPage.DisplayAlert("Помилка", "Завантаження з галереї підтримується лише на Android.", "OK");
#endif
        }
        catch (Exception ex)
        {
            Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
        }
    }
}