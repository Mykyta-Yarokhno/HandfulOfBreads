using CommunityToolkit.Maui.Views;
using HandfulOfBreads.Services;

namespace HandfulOfBreads.Views.Popups;

public partial class NewPatternPopup : Popup
{
    private readonly NewDesignStartPage _newDesignStartPage;

    public LocalizationResourceManager LocalizationResourceManager
       => LocalizationResourceManager.Instance;

    public NewPatternPopup(NewDesignStartPage newDesignStartPage)
    {
        InitializeComponent();

        _newDesignStartPage = newDesignStartPage;
    }

    private async void ConvertPhoto_Clicked(object sender, EventArgs e)
    {
        Close("ConvertPhoto");
    }

    private async void DrawNewDesign_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.Navigation.PushAsync(_newDesignStartPage);
        Close();
    }

    private async void ImportFile_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}