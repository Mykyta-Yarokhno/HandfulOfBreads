using HandfulOfBreads.ViewModels;

namespace HandfulOfBreads.Views;

public partial class StartPage : ContentPage
{
	public StartPage(StartPageViewModel viewModel)
	{
        InitializeComponent();
        BindingContext = viewModel;
        Shell.SetNavBarIsVisible(this, false);

        LoadPixelGrids();
    }

    private void LoadPixelGrids()
    {
        try
        {
#if ANDROID
            var picturesPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
            var pixelGridFiles = Directory.GetFiles(picturesPath)
                                           .Where(file => Path.GetFileName(file).StartsWith("pixel_grid_"))
                                           .OrderByDescending(file => File.GetLastWriteTime(file))
                                           .ToList();

            stackLayout.Children.Clear();

            foreach (var file in pixelGridFiles)
            {
                var image = new Image
                {
                   Source = ImageSource.FromFile(file),
                    Aspect = Aspect.AspectFit,
                    WidthRequest = 150,
                    HeightRequest = 150
                };

                var frame = new Frame
                {
                    Content = image,
                    Margin = new Thickness(10),
                    WidthRequest = 154,
                    HeightRequest = 154,
                    CornerRadius = 0,
                    HasShadow = false,
                    BorderColor = Color.FromArgb("#553d3a"),
                    BackgroundColor = Colors.Wheat
                };

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