using HandfulOfBreads.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HandfulOfBreads.ViewModels;

public partial class ColorItemViewModel : BaseViewModel
{
    public ColorItem Model { get; }

    public ColorItemViewModel(ColorItem model)
    {
        Model = model;
    }

    public string Code => Model.Code;
    public string HexColor => Model.HexColor;
    public char? Sign => Model.Sign;

    [ObservableProperty]
    private bool isSelected;

}

