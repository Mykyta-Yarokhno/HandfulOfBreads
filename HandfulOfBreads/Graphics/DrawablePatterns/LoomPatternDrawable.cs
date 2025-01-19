using SkiaSharp;
using System.Reflection;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace HandfulOfBreads.Graphics.DrawablePatterns
{
    internal class LoomPatternDrawable : IPatternDrawable
    {
        private readonly List<List<bool>> _grid = new();
        private int _rows;
        private int _columns;
        private int _pixelSize;

        private IImage? _fillImage;

        public void InitializeGrid(int rows, int columns, int pixelSize, IImage? fillImage = null)
        {
            _rows = rows;
            _columns = columns;
            _pixelSize = pixelSize;

            _fillImage = fillImage;

            _grid.Clear();

            for (int i = 0; i < rows; i++)
            {
                var row = new List<bool>();

                for (int j = 0; j < columns; j++)
                {
                    row.Add(false);
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

                    if (_grid[row][col])
                    {
                        if (_fillImage != null)
                        {
                            canvas.DrawImage(_fillImage, x, y, _pixelSize, _pixelSize);
                        }
                        else
                        {
                            canvas.FillColor = Colors.Black;
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
                if (!_grid[row][col])
                {
                    _grid[row][col] = true;
                }
            }
        }

        private SKBitmap? LoadBitmapFromResource(string resourcePath)
        {
            var assembly = GetType().GetTypeInfo().Assembly;
            using var stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null) return null;

            return SKBitmap.Decode(stream);
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

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    float x = col * _pixelSize;
                    float y = row * _pixelSize;

                    if (_grid[row][col])
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
                                Color = SKColors.Black,
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

