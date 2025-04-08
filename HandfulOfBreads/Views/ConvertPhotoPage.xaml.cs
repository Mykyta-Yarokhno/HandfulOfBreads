using HandfulOfBreads.ViewModels;

namespace HandfulOfBreads.Views;

public partial class ConvertPhotoPage : ContentPage
{
    public ConvertPhotoPage()
    {
        InitializeComponent();
        BindingContext = new ConvertPhotoViewModel();
    }
}