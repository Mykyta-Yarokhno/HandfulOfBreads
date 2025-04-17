using CommunityToolkit.Maui.Views;

namespace HandfulOfBreads.Views.Popups;

public partial class ChoosePhotoPopup : Popup
{
    public List<GalleryImage> Images { get; set; } = new();
    private GalleryImage _selectedImage;
    public ChoosePhotoPopup(List<GalleryImage> images)
	{
        Images = images;
        
        InitializeComponent();

        BindingContext = this;
    }

    //private void OnImageSelected(object sender, SelectionChangedEventArgs e)
    //{
    //    _selectedImage = e.CurrentSelection.FirstOrDefault() as GalleryImage;
    //}

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close(null);
    }

    private async void OnOkClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.Navigation.PushAsync(new ImageToGridPage(_selectedImage?.FullPath));
        Close();
    }

    public class GalleryImage
    {
        public string FullPath { get; set; }
        public ImageSource Thumbnail => ImageSource.FromFile(FullPath);
        public bool IsSelected { get; set; }
    }

    private void OnImageTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is GalleryImage tappedImage)
        {
            foreach (var img in Images)
                img.IsSelected = false;

            tappedImage.IsSelected = true;
            _selectedImage = tappedImage;

            ImagesView.ItemsSource = null;
            ImagesView.ItemsSource = Images;
        }
    }
}