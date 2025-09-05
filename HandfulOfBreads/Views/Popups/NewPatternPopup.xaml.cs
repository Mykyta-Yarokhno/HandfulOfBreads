using CommunityToolkit.Maui.Views;
using HandfulOfBreads.Services;

namespace HandfulOfBreads.Views.Popups;

public partial class NewPatternPopup : Popup
{
    public LocalizationResourceManager LocalizationResourceManager => LocalizationResourceManager.Instance;

    public NewPatternPopup()
    {
        InitializeComponent();
        Opened += OnPopupOpened;
    }

    private void OnPopupOpened(object? sender, EventArgs e)
    {
        if (PopupFrame != null)
        {
            PopupFrame.Scale = 0;

            PopupFrame.AnchorX = 0.5;
            PopupFrame.AnchorY = 0.5;

            PopupFrame.ScaleTo(1, 250, Easing.CubicOut);
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
            PopupFrame.AnchorX = 0.5;
            PopupFrame.AnchorY = 0.5;

            await PopupFrame.ScaleTo(0, 250, Easing.CubicIn);
        }

        Close(result);
    }

    private void ConvertPhoto_Clicked(object sender, EventArgs e)
    {
        CloseWithAnimation("ConvertPhoto");
    }

    private async void DrawNewDesign_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(NewDesignStartPage));
        CloseWithAnimation();
    }

    private void ImportFile_Clicked(object sender, EventArgs e)
    {
        CloseWithAnimation();
    }
}