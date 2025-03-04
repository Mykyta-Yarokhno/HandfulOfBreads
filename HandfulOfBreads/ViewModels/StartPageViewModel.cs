using HandfulOfBreads.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Views;
using HandfulOfBreads.Views.Popups;

namespace HandfulOfBreads.ViewModels
{
    public class StartPageViewModel
    {
        private readonly ImageLoadingService _imageLoadingService;
        public ObservableCollection<ImageModel> Images { get; set; } = new ObservableCollection<ImageModel>();
        public ICommand AddImageCommand { get; set; }

        public StartPageViewModel(ImageLoadingService imageLoadingService)
        {
            _imageLoadingService = imageLoadingService;
            AddImageCommand = new Command(async () => await AddImageAsync());
            LoadImagesAsync();
        }

        private async Task LoadImagesAsync()
        {
            var images = await _imageLoadingService.LoadSavedImagesAsync(40);
            foreach (var image in images)
            {
                Images.Add(image);
            }
            await Task.Delay(100);
            DisplayImages();
        }

        public void DisplayImages()
        {
            if (Shell.Current?.CurrentPage?.FindByName<FlexLayout>("ImagesFlexLayout") is FlexLayout flexLayout)
            {
                System.Diagnostics.Debug.WriteLine($"FlexLayout found: {flexLayout.Id}");
                flexLayout.Children.Clear();

                foreach (var imageModel in Images)
                {
                    var imageButton = new ImageButton
                    {
                        Source = imageModel.Thumbnail,
                        Aspect = Aspect.AspectFill,
                        WidthRequest = 100,
                        HeightRequest = 100,
                        Margin = 5
                    };
                    flexLayout.Children.Add(imageButton);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ImagesFlexLayout not found.");
            }
        }

        private async Task AddImageAsync()
        {
            var popup = new NewPatternPopup();
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        }
    }
}