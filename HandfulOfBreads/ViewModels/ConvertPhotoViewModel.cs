using Microsoft.Maui;
using Microsoft.Maui.Controls;
using SkiaSharp;
using System.IO;
using System.Windows.Input;

namespace HandfulOfBreads.ViewModels
{
    public class ConvertPhotoViewModel
    {
        public MemoryStream ImageData { get; set; } = new MemoryStream();

        public ICommand SelectPhotoCommand { get; }
        public ICommand ConvertToGridCommand { get; }

        public ConvertPhotoViewModel()
        {
            SelectPhotoCommand = new Command(async () => await SelectPhoto());
            ConvertToGridCommand = new Command(ConvertToGrid);
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
                // Обробка помилок
            }
        }

        private void ConvertToGrid()
        {
            if (ImageData.Length > 0)
            {
                using (SKBitmap bitmap = SKBitmap.Decode(ImageData.ToArray()))
                {
                    int pixelSize = 10;
                    int columns = bitmap.Width / pixelSize;
                    int rows = bitmap.Height / pixelSize;

                    Color[][] grid = new Color[rows][];
                    for (int row = 0; row < rows; row++)
                    {
                        grid[row] = new Color[columns];
                        for (int col = 0; col < columns; col++)
                        {
                            int x = col * pixelSize;
                            int y = row * pixelSize;
                            SKColor pixelColor = bitmap.GetPixel(x, y);
                            grid[row][col] = Color.FromRgba(pixelColor.Red, pixelColor.Green, pixelColor.Blue, pixelColor.Alpha);
                        }
                    }
                    MainThread.BeginInvokeOnMainThread(() => DisplayGrid(grid, columns, rows));

                    Shell.Current?.CurrentPage?.DisplayAlert("Success", "Image converted to grid!", "OK");
                }
            }
            else
            {
                Shell.Current?.CurrentPage?.DisplayAlert("Error", "Please select an image first.", "OK");
            }
        }

        private void DisplayGrid(Color[][] grid, int columns, int rows)
        {
            if (Shell.Current?.CurrentPage?.FindByName("GridDisplay") is Grid gridDisplay)
            {
                gridDisplay.Children.Clear();
                gridDisplay.RowDefinitions.Clear();
                gridDisplay.ColumnDefinitions.Clear();

                for (int row = 0; row < rows; row++)
                {
                    gridDisplay.RowDefinitions.Add(new RowDefinition(GridLength.Star));
                }

                for (int col = 0; col < columns; col++)
                {
                    gridDisplay.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                }

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < columns; col++)
                    {
                        var boxView = new BoxView { Color = grid[row][col] };
                        Grid.SetRow(boxView, row);
                        Grid.SetColumn(boxView, col);
                        gridDisplay.Children.Add(boxView);
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("GridDisplay not found.");
            }
        }
    }
}