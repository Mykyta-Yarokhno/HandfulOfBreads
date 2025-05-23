using HandfulOfBreads.ViewModels;
using CommunityToolkit.Maui.Views;

namespace HandfulOfBreads.Views.Popups;

public partial class SearchColorPopup : Popup
{
    public event Action<ColorItemViewModel>? ColorSelected;
    private List<ColorItemViewModel> _allColors;

    public SearchColorPopup(List<ColorItemViewModel> colors)
    {
        InitializeComponent();
        _allColors = colors;
        BindingContext = this;
        UpdateResults(colors);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                UpdateResults(_allColors
                    .Where(c => c.Code.Contains(_searchText, StringComparison.OrdinalIgnoreCase))
                    .ToList());
            }
        }
    }
    private string _searchText = string.Empty;

    public Command PerformSearchCommand => new(() =>
    {
        UpdateResults(_allColors
            .Where(c => c.Code.Contains(_searchText, StringComparison.OrdinalIgnoreCase))
            .ToList());
    });

    private void UpdateResults(List<ColorItemViewModel> filteredColors)
    {
        ResultsContainer.Children.Clear();

        foreach (var color in filteredColors)
        {
            var label = new Label
            {
                Text = color.Code,
                FontSize = 16,
                BackgroundColor = Colors.LightGray,
                Padding = 10
            };

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                ColorSelected?.Invoke(color);
                Close();
            };
            label.GestureRecognizers.Add(tap);

            ResultsContainer.Children.Add(label);
        }
    }
}
