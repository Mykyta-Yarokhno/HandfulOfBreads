namespace HandfulOfBreads.Views;

public partial class StartPage : ContentPage
{
	public StartPage()
	{
		InitializeComponent();
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
            return number >= 0 && number <= 100;
        }

        return false;
    }

    private async void OnOkButtonClicked(object sender, EventArgs e)
    {
        int firstNumber = int.Parse(FirstNumberEntry.Text);
        int secondNumber = int.Parse(SecondNumberEntry.Text);

        await Navigation.PushModalAsync(new MainPage(firstNumber, secondNumber));
    }
}