using HandfulOfBreads.Services;
using HandfulOfBreads.ViewModels;

namespace HandfulOfBreads.Views;

public partial class StartPage : ContentPage
{
    private StartPageViewModel _viewModel;

    public StartPage(StartPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    //protected override void OnAppearing()
    //{
    //    base.OnAppearing();
    //    _viewModel.DisplayImages();
    //}
}