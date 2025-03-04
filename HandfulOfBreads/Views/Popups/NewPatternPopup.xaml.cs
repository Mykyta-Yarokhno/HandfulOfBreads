using CommunityToolkit.Maui.Views;

namespace HandfulOfBreads.Views.Popups;

public partial class NewPatternPopup : Popup
{
    public NewPatternPopup()
    {
        InitializeComponent();
    }

    private async void ConvertPhoto_Clicked(object sender, EventArgs e)
    {
        Close();
        await Shell.Current.GoToAsync(nameof(ConvertPhotoPage));
    }

    private async void DrawDesign_Clicked(object sender, EventArgs e)
    {
        Close();
        await Shell.Current.GoToAsync(nameof(NewDesignStartPage));
    }

    private async void ImportFile_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}