using HandfulOfBreads.ViewModels;

namespace HandfulOfBreads.Views;

public partial class NewDesignStartPage : ContentPage
{
    public NewDesignStartPage(NewDesignStartViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        Shell.SetNavBarIsVisible(this, false);
    }
}