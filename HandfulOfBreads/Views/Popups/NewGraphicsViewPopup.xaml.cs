using CommunityToolkit.Maui.Views;
using HandfulOfBreads.Services;

namespace HandfulOfBreads.Views.Popups;

public partial class NewGraphicsViewPopup : Popup
{
    public event EventHandler<StartPopupResultEventArgs> ResultReady;

    private int DropValue { get; set; } = 1;

    public NewGraphicsViewPopup()
    {
        InitializeComponent();
        InitializeUi();
    }

    private void InitializeUi()
    {
        PatternsPicker.Items.Add("Loom");
        PatternsPicker.Items.Add("Brick");
        PatternsPicker.Items.Add("Peyote");
        PatternsPicker.SelectedIndex = 0;

        for (int i = 1; i <= 5; i++)
        {
            DropPicker.Items.Add(i.ToString());
        }
        DropPicker.SelectedIndex = 0;

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

    private void DropPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DropPicker.SelectedItem is string selectedDrop)
        {
            if (int.TryParse(selectedDrop, out int drop))
            {
                DropValue = drop;
            }
        }
    }

    private void PatternsPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        string selectedPattern = picker.SelectedItem.ToString();

        if (selectedPattern == "Peyote")
        {
            DropLabel.IsVisible = true;
            DropPicker.IsVisible = true;
            if (DropPicker.SelectedItem is string selectedDrop && int.TryParse(selectedDrop, out int drop))
            {
                DropValue = drop;
            }
        }
        else
        {
            DropLabel.IsVisible = false;
            DropPicker.IsVisible = false;
            DropValue = 1;
        }

        if (selectedPattern == "Brick")
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
        public int Drop { get; }

        public StartPopupResultEventArgs(int columns, int rows, string selectedPattern, int drop)
        {
            Columns = columns;
            Rows = rows;
            SelectedPattern = selectedPattern;
            Drop = drop;
        }
    }

    private async void OkButton_Clicked(object sender, EventArgs e)
    {
        int columns = int.Parse(ColumnsEntry.Text);
        int rows = int.Parse(RowsEntry.Text);
        string selectedPattern = PatternsPicker.SelectedItem.ToString();
        int drop = selectedPattern == "Peyote" ? DropValue : 1;

        var navigationParameters = new Dictionary<string, object>
        {
            { "Columns", columns },
            { "Rows", rows },
            { "SelectedPattern", selectedPattern },
            { "Drop", drop }
        };

        ResultReady?.Invoke(this, new StartPopupResultEventArgs(columns, rows, selectedPattern, drop));

        Close();

        //await Shell.Current.GoToAsync(nameof(MainPage), navigationParameters);
    }
}