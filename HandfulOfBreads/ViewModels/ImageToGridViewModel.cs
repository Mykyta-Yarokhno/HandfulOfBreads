using HandfulOfBreads.Views;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HandfulOfBreads.ViewModels
{
    public class ImageToGridViewModel :BaseViewModel
    {
        private readonly string _filepath;

        private int _width = 150;
        public int Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        private int _height = 150;
        public int Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        private string _selectedPattern = "Peyote";
        public string SelectedPattern
        {
            get => _selectedPattern;
            set => SetProperty(ref _selectedPattern, value);
        }

        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        public ObservableCollection<string> Patterns { get; } = new() { "Peyote", "Brick", "Loom" };

        private List<List<Color>>? _colorGrid;

        public ICommand GenerateGridCommand { get; }
        public ICommand GoToMainPageCommand { get; }
        public ImageToGridViewModel(string filepath)
        {
            _filepath = filepath;

            ImageSource = ImageSource.FromFile(filepath);

            GenerateGridCommand = new Command(async () => await GenerateGrid());
            GoToMainPageCommand = new Command(async () => await GoToMainPage());
        }

        private async Task GoToMainPage()
        {
            Application.Current.MainPage.Navigation.PushAsync(new MainPage(150 , 150,"Loom", _colorGrid));
        }

        private async Task GenerateGrid()
        {
            var bitmap = await LoadBitmapAsync(_filepath);

            _colorGrid = new();
            for (int y = 0; y < Height; y++)
            {
                var row = new List<Color>();
                for (int x = 0; x < Width; x++)
                {
                    int px = x * 40;
                    int py = y * 40;

                    if (px >= bitmap.Width || py >= bitmap.Height)
                    {
                        row.Add(Colors.Transparent);
                    }
                    else
                    {
                        var pixel = bitmap.GetPixel(px, py);
                        var mauiColor = Color.FromRgba(pixel.Red, pixel.Green, pixel.Blue, pixel.Alpha);
                        row.Add(mauiColor); 
                    }
                }
                _colorGrid.Add(row);
            }
        }

        private async Task<SKBitmap> LoadBitmapAsync(string path)
        {
            using var stream = File.OpenRead(path);

            var codec = SKCodec.Create(stream);
            if (codec == null)
                throw new Exception("Не вдалося зчитати зображення");

            var info = codec.Info;
            var bitmap = new SKBitmap(info.Width, info.Height);
            var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels());

            // Коригуємо орієнтацію
            var oriented = SKBitmap.Decode(path).WithOrigin(codec.EncodedOrigin);
            return oriented;
        }
         

    }

    public static class SkiaExtensions
    {
        public static SKBitmap WithOrigin(this SKBitmap bitmap, SKEncodedOrigin origin)
        {
            SKBitmap rotated;

            switch (origin)
            {
                case SKEncodedOrigin.RightTop:
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.RotateDegrees(90, rotated.Width / 2f, rotated.Height / 2f);
                        canvas.Translate(rotated.Width - bitmap.Width, 0);
                        canvas.DrawBitmap(bitmap, 0, 0);
                    }
                    break;

                case SKEncodedOrigin.BottomRight:
                    rotated = new SKBitmap(bitmap.Width, bitmap.Height);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.RotateDegrees(180, rotated.Width / 2f, rotated.Height / 2f);
                        canvas.DrawBitmap(bitmap, 0, 0);
                    }
                    break;

                case SKEncodedOrigin.LeftBottom:
                    rotated = new SKBitmap(bitmap.Height, bitmap.Width);
                    using (var canvas = new SKCanvas(rotated))
                    {
                        canvas.RotateDegrees(270, rotated.Width / 2f, rotated.Height / 2f);
                        canvas.Translate(0, rotated.Height - bitmap.Height);
                        canvas.DrawBitmap(bitmap, 0, 0);
                    }
                    break;

                default:
                    return bitmap;
            }

            return rotated;
        }
    }
}
