using CommunityToolkit.Maui.Views;
using HandfulOfBreads.Services;

namespace HandfulOfBreads.Views.Popups;

public partial class NewPatternPopup : Popup
{
    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

    public NewPatternPopup()
    {
        InitializeComponent();
    }

    private async void ConvertPhoto_Clicked(object sender, EventArgs e)
    {
        Close();
    }

    private async void DrawNewDesign_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.Navigation.PushAsync(new NewDesignStartPage());
        Close();
    }

    private async void ImportFile_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}