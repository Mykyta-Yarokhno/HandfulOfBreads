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

    private void OnOkClicked(object sender, EventArgs e)
    {
        if (int.TryParse(FirstNumberEntry.Text, out int firstNumber) &&
            int.TryParse(SecondNumberEntry.Text, out int secondNumber))
        {
            FirstNumber = firstNumber;
            SecondNumber = secondNumber;

            Close();
        }
        //else
        //{
        //    DisplayAlert("Error", "Please enter valid numbers.", "OK");
        //}   
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close();
    }
}