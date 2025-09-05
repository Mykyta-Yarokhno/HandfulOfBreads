using HandfulOfBreads.ViewModels;

namespace HandfulOfBreads.Views;

public partial class StartPage : ContentPage
{
    public StartPage(StartPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        Shell.SetNavBarIsVisible(this, false);
        
        DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateLayout();
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        DeviceDisplay.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;
    }
    
    private void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        UpdateLayout();
    }
    
    // Main update logic
    private void UpdateLayout()
    {
        var screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        
        int spanCount;
        
        if (DeviceInfo.Current.Idiom == DeviceIdiom.Phone)
        {
            if (DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Portrait)
            {
                spanCount = 2;
            }
            else
            {
                spanCount = 3;
            }
        }
        else
        {
            spanCount = 4;
        }
        
        galleryCollectionView.ItemsLayout = new GridItemsLayout(spanCount, ItemsLayoutOrientation.Vertical)
        {
            HorizontalItemSpacing = 10,
            VerticalItemSpacing = 10
        };
    }
}