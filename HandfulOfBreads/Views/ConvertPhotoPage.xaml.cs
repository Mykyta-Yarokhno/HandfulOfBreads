using HandfulOfBreads.ViewModels;

namespace HandfulOfBreads.Views;

public partial class ConvertPhotoPage : ContentPage
{
    public ConvertPhotoPage()
    {
        InitializeComponent();
        BindingContext = new ConvertPhotoViewModel(); // Встановлення BindingContext
    }

    private async void ConvertToGrid_Clicked(object sender, EventArgs e)
    {

    }
}