using CommunityToolkit.Maui.Views;

namespace HandfulOfBreads.Views.Popups;

public partial class NewGraphicsViewPopup : Popup
{
    public int FirstNumber { get; set; }
    public int SecondNumber { get; set; }
    public NewGraphicsViewPopup()
    {
        InitializeComponent();
    }

    private async void OnOkClicked(object sender, EventArgs e)
    {
        if (int.TryParse(FirstNumberEntry.Text, out int firstNumber) &&
            int.TryParse(SecondNumberEntry.Text, out int secondNumber))
        {
            // Close the popup first to prevent UI conflicts
            Close();

            // Create a dictionary of parameters to pass to MainPage
            var navigationParameters = new Dictionary<string, object>
        {
            { "Columns", firstNumber },
            { "Rows", secondNumber },
            { "SelectedPattern", "Loom" }
        };

            // Use Shell navigation to go to MainPage with the parameters
            await Shell.Current.GoToAsync(nameof(MainPage), navigationParameters);
        }
        else
        {
            // It's good practice to provide user feedback
            //await DisplayAlert("Error", "Please enter valid numbers.", "OK");
        }
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close();
    }
}