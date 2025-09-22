using CommunityToolkit.Maui.Views;
using HandfulOfBreads.Services;

namespace HandfulOfBreads.Views.Popups;

public partial class NewGraphicsViewPopup : Popup
{
    public event EventHandler<StartPopupResultEventArgs> ResultReady;

    public NewGraphicsViewPopup()
    {
        InitializeComponent();
        InitializeUi();
    }

    private void InitializeUi()
    {
        PatternsPicker.Items.Add("Loom");
        PatternsPicker.Items.Add("Brick");
        PatternsPicker.Items.Add("Payote");
        PatternsPicker.SelectedIndex = 0;

        RowsEntry.Text = "20";
        ColumnsEntry.Text = "10";
        ValidateInput();
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidateInput();
    }

    private void ValidateInput()
    {
        bool isValid = int.TryParse(ColumnsEntry.Text, out int c) && int.TryParse(RowsEntry.Text, out int r)
                       && c >= 0 && c <= 200 && r >= 0 && r <= 200;
        OkButton.IsEnabled = isValid;
    }

    private void PatternsPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        string selectedPattern = picker.SelectedItem.ToString();
        if (selectedPattern == "Payote" || selectedPattern == "Brick")
        {
            Application.Current.MainPage.DisplayAlert("Unavailable", "This option is currently disabled.", "OK");
            picker.SelectedItem = "Loom";
        }
    }

    public class StartPopupResultEventArgs : EventArgs
    {
        public int Columns { get; }
        public int Rows { get; }
        public string SelectedPattern { get; }

        public StartPopupResultEventArgs(int columns, int rows, string selectedPattern)
        {
            Columns = columns;
            Rows = rows;
            SelectedPattern = selectedPattern;
        }
    }

    private async void OkButton_Clicked(object sender, EventArgs e)
    {
        int columns = int.Parse(ColumnsEntry.Text);
        int rows = int.Parse(RowsEntry.Text);
        string selectedPattern = PatternsPicker.SelectedItem.ToString();

        var navigationParameters = new Dictionary<string, object>
        {
            { "Columns", columns },
            { "Rows", rows },
            { "SelectedPattern", selectedPattern }
        };

        ResultReady?.Invoke(this, new StartPopupResultEventArgs(columns, rows, selectedPattern));

        Close();

        //await Shell.Current.GoToAsync(nameof(MainPage), navigationParameters);
    }
}