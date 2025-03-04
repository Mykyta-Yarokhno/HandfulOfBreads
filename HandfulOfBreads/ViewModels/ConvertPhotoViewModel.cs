using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HandfulOfBreads.ViewModels;

public class ConvertPhotoViewModel
{
    public MemoryStream ImageData { get; set; } = new MemoryStream(); 

    public ICommand SelectPhotoCommand { get; }

    public ConvertPhotoViewModel()
    {
        SelectPhotoCommand = new Command(async () => await SelectPhoto());
    }

    private async Task SelectPhoto()
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync();
            if (result != null)
            {
                ImageData.SetLength(0);
                using (var stream = await result.OpenReadAsync())
                {
                    await stream.CopyToAsync(ImageData);
                }
                ImageData.Position = 0; 
            }
        }
        catch (Exception ex)
        {
            
        }
    }
}