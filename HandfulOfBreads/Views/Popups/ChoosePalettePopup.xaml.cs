using CommunityToolkit.Maui.Views;

namespace HandfulOfBreads.Views.Popups;

public partial class ChoosePalettePopup : Popup
{
    public event Action<string>? PaletteSelected;
    private readonly string? _currentPalette;
    public ChoosePalettePopup(string? currentPaletteName = null)
    {
        InitializeComponent();
        _currentPalette = currentPaletteName;
        var paletteNames = new List<string>
        {
            "Preciosa Rocialles",
            "ass we can",
            "Used Ñolours"
        };

        foreach (var name in paletteNames)
        {
            var button = new Button
            {
                Text = name != currentPaletteName ? name : name + " !",
                Style = (Style)Application.Current.Resources["MainButton"]
            };

            button.Clicked += (s, e) => OnPaletteClicked(name);

            ButtonsContainer.Children.Add(button);
        }
    }

    private void OnPaletteClicked(string paletteName)
    {
        if (paletteName != _currentPalette)
            PaletteSelected?.Invoke(paletteName);

        Close();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        Close();
    }
}