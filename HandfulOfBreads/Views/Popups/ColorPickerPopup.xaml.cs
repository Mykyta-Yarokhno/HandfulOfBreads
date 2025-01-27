using CommunityToolkit.Maui.Views;
using SkiaSharp;
using System.Reflection;

namespace HandfulOfBreads.Views.Popups;

public partial class ColorPickerPopup : Popup
{
    private SKBitmap? _paletteBitmap;

    public ColorPickerPopup()
    {
        InitializeComponent();
        LoadBitmapFromResource();
    }

    private void LoadBitmapFromResource()
    {
        try
        {
            var resourcePath = "HandfulOfBreads.Resources.Images.color_palette.png";
            var assembly = GetType().GetTypeInfo().Assembly;

            using var stream = assembly.GetManifestResourceStream(resourcePath);

            if (stream == null)
            {
                Console.WriteLine($"Resource not found at path: {resourcePath}");
                return;
            }

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            memoryStream.Seek(0, SeekOrigin.Begin);

            PaletteImage.Source = ImageSource.FromStream(() => new MemoryStream(memoryStream.ToArray()));

            memoryStream.Seek(0, SeekOrigin.Begin);

            _paletteBitmap = SKBitmap.Decode(memoryStream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading resource: {ex.Message}");
            _paletteBitmap = null;
        }
    }

    private void OnPaletteTapped(object sender, TappedEventArgs e)
    {
        if (_paletteBitmap == null || e.GetPosition(PaletteImage) is not Point touchPoint)
            return;

        var pixelX = (int)(touchPoint.X / PaletteImage.Width * _paletteBitmap.Width);
        var pixelY = (int)(touchPoint.Y / PaletteImage.Height * _paletteBitmap.Height);

        if (pixelX >= 0 && pixelX < _paletteBitmap.Width &&
            pixelY >= 0 && pixelY < _paletteBitmap.Height)
        {
            var pixelColor = _paletteBitmap.GetPixel(pixelX, pixelY);
            var selectedColor = Color.FromRgb(pixelColor.Red, pixelColor.Green, pixelColor.Blue);

            PreviewBox.BackgroundColor = selectedColor;
        }
    }

    private void OnConfirmClicked(object sender, EventArgs e)
    {
        Close(PreviewBox.BackgroundColor);
    }
}