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
}