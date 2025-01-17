namespace HandfulOfBreads.Views;

public partial class NewGraphicsViewPageModal : ContentPage
{
    public event EventHandler<(int firstNumber, int secondNumber)> Confirmed;

    public NewGraphicsViewPageModal()
    {
        InitializeComponent();
    }

    private void OnOkClicked(object sender, EventArgs e)
    {
        if (int.TryParse(FirstNumberEntry.Text, out int firstNumber) &&
            int.TryParse(SecondNumberEntry.Text, out int secondNumber))
        {
            Confirmed?.Invoke(this, (firstNumber, secondNumber));
            CloseModal();
        }
        else
        {
            DisplayAlert("Error", "Please enter valid numbers.", "OK");
        }
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        CloseModal();
    }

    private async void CloseModal()
    {
        await Navigation.PopModalAsync();
    }
}