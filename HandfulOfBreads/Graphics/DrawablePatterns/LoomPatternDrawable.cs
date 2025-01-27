using SkiaSharp;
using System.Reflection;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace HandfulOfBreads.Graphics.DrawablePatterns
{
    internal class LoomPatternDrawable : IPatternDrawable
    {
        private readonly List<List<Color>> _grid = new();
        private int _rows;
        private int _columns;
        private int _pixelSize;

        private IImage? _fillImage;

        private Color _selectedColor = Colors.White;

        public Color SelectedColor
        {
            get => _selectedColor;
            set => _selectedColor = value;
        }

        public void InitializeGrid(int rows, int columns, int pixelSize, IImage? fillImage = null)
        {
            _rows = rows;
            _columns = columns;
            _pixelSize = pixelSize;

            _fillImage = null;

            _grid.Clear();

            for (int i = 0; i < rows; i++)
            {
                var row = new List<Color>();
                for (int j = 0; j < columns; j++)
                {
                    row.Add(Colors.Transparent);
                }
                _grid.Add(row);
            }
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.White;
            canvas.FillRectangle(dirtyRect);

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    float x = col * _pixelSize;
                    float y = row * _pixelSize;

                    if (_grid[row][col] != Colors.Transparent)
                    {
                        if (_fillImage != null)
                        {
                            canvas.DrawImage(_fillImage, x, y, _pixelSize, _pixelSize);
                        }
                        else
                        {
                            canvas.FillColor = _grid[row][col];
                            canvas.FillRectangle(x, y, _pixelSize, _pixelSize);
                        }
                    }

                    canvas.StrokeColor = Colors.Gray;
                    canvas.StrokeSize = 1;
                    canvas.DrawRectangle(x, y, _pixelSize, _pixelSize);
                }
            }
        }

        public void TogglePixel(float x, float y)
        {
            int col = (int)(x / _pixelSize);
            int row = (int)(y / _pixelSize);

            if (row >= 0 && row < _rows && col >= 0 && col < _columns)
            {
                _grid[row][col] = _selectedColor;
            }
        }

        private SKBitmap? LoadBitmapFromResource(string resourcePath)
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            using var stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null) return null;

            return SKBitmap.Decode(stream);
        }

        private SKColor ConvertToSKColor(Color color)
        {
            return new SKColor(
                (byte)(color.Red * 255),
                (byte)(color.Green * 255),
                (byte)(color.Blue * 255),
                (byte)(color.Alpha * 255)
            );
        }

        public async Task SaveToFileAsync(string filePath)
        {
            int width = _columns * _pixelSize;
            int height = _rows * _pixelSize;

            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            var resourcePath = "HandfulOfBreads.Resources.Images.bonk.png";
            var _fillImage2 = LoadBitmapFromResource(resourcePath);
            _fillImage2 = null;

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    float x = col * _pixelSize;
                    float y = row * _pixelSize;

                    if (_grid[row][col] != Colors.Transparent)
                    {
                        if (_fillImage2 != null && _fillImage2 is SKBitmap bitmap)
                        {
                            var rect = new SKRect(x, y, x + _pixelSize, y + _pixelSize);
                            canvas.DrawBitmap(bitmap, rect);
                        }
                        else
                        {
                            using var paint = new SKPaint
                            {
                                Color = ConvertToSKColor(_grid[row][col]),
                                Style = SKPaintStyle.Fill
                            };
                            canvas.DrawRect(x, y, _pixelSize, _pixelSize, paint);
                        }
                    }

                    using var strokePaint = new SKPaint
                    {
                        Color = SKColors.Gray,
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = 1
                    };
                    canvas.DrawRect(x, y, _pixelSize, _pixelSize, strokePaint);
                }
            }

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            using (var stream = File.OpenWrite(filePath))
            {
                data.SaveTo(stream);
            }
        }
    }
}

