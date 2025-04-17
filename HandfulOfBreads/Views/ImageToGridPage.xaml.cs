using HandfulOfBreads.ViewModels;

namespace HandfulOfBreads.Views;

public partial class ImageToGridPage : ContentPage
{
	public ImageToGridPage(string filepath)
	{
		InitializeComponent();
        BindingContext = new ImageToGridViewModel(filepath); ;
        Shell.SetNavBarIsVisible(this, false);
    }
}