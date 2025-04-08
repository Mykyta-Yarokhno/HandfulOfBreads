using System.Text.Json;
using System.Text;

namespace HandfulOfBreads.Views;

public partial class NewDesignStartPage : ContentPage
{
    public NewDesignStartPage()
    {
        InitializeComponent();
        Shell.SetNavBarIsVisible(this, false);
    }

    private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        bool isFirstValid = IsValidNumber(FirstNumberEntry.Text);
        bool isSecondValid = IsValidNumber(SecondNumberEntry.Text);

        OkButton.IsEnabled = isFirstValid && isSecondValid;
    }

    private bool IsValidNumber(string? input)
    {
        if (int.TryParse(input, out int number))
        {
            return number >= 0 && number <= 200;
        }

        return false;
    }

    private void OnPatternPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (/*PatternPicker.SelectedIndex == 1 ||*/ PatternPicker.SelectedIndex == 2)
        {
            DisplayAlert("Unavailable", "This option is currently disabled.", "OK");
            PatternPicker.SelectedIndex = 0;
        }
    }

    private async void OnOkButtonClicked(object sender, EventArgs e)
    {

        string selectedPattern = PatternPicker.SelectedItem?.ToString();

        if (string.IsNullOrWhiteSpace(selectedPattern))
        {
            DisplayAlert("Error", "Please select a pattern.", "OK");
            return;
        }

        int firstNumber = int.Parse(FirstNumberEntry.Text);
        int secondNumber = int.Parse(SecondNumberEntry.Text);

        await Navigation.PushModalAsync(new MainPage(firstNumber, secondNumber, selectedPattern));
    }

    private async void OnOpenExistingButtonClicked(object sender, EventArgs e)
    {
        try
        {
            var fileResult = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Оберіть ваш збережений малюнок",
                FileTypes = FilePickerFileType.Images
            });

            if (fileResult == null)
                return;

            var filePath = fileResult.FullPath;

            if (!Path.GetFileName(filePath).StartsWith("pixel_grid_"))
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", "Цей файл не є малюнком програми.", "OK");
                return;
            }

            var (name, rows, columns, pixelSize, grid) = await LoadGridFromFileAsync(filePath);

            await Navigation.PushModalAsync(new MainPage(rows, columns, name, grid));
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
        }
    }

    public async Task<(string name, int rows, int columns, int pixelSize, List<List<Color>> grid)> LoadGridFromFileAsync(string filePath)
    {
        byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
        string fileText = Encoding.UTF8.GetString(fileBytes);

        int gridStart = fileText.IndexOf("<GRID>");
        int gridEnd = fileText.IndexOf("</GRID>");

        if (gridStart == -1 || gridEnd == -1)
            throw new Exception("Не знайдено JSON-метадані у файлі.");

        string json = fileText.Substring(gridStart + "<GRID>".Length, gridEnd - gridStart - "<GRID>".Length).Trim();
        var meta = JsonSerializer.Deserialize<GridMeta>(json);

        if (meta is null)
            throw new Exception("Не вдалося розпарсити JSON.");

        var grid = meta.grid
            .Select(row => row.Select(hex => FromHex(hex)).ToList())
            .ToList();

        return (meta.name,meta.rows, meta.columns, meta.pixelSize, grid);
    }
    public Color FromHex(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex)) return Colors.Transparent;

        if (hex.StartsWith("#"))
            hex = hex[1..];

        if (hex.Length == 6) // RRGGBB
        {
            byte r = Convert.ToByte(hex[..2], 16);
            byte g = Convert.ToByte(hex[2..4], 16);
            byte b = Convert.ToByte(hex[4..6], 16);
            return Color.FromRgb(r, g, b);
        }
        else if (hex.Length == 8) // AARRGGBB
        {
            byte a = Convert.ToByte(hex[..2], 16);
            byte r = Convert.ToByte(hex[2..4], 16);
            byte g = Convert.ToByte(hex[4..6], 16);
            byte b = Convert.ToByte(hex[6..8], 16);
            return Color.FromRgba(r, g, b, a);
        }

        return Colors.Transparent;
    }

    public class GridMeta
    {
        public string name { get; set; }
        public int rows { get; set; }
        public int columns { get; set; }
        public int pixelSize { get; set; }
        public List<List<string>> grid { get; set; }
    }

}