using SkiaSharp;
using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
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

        public void InitializeGrid(int rows, int columns, int pixelSize, IImage? fillImage = null, List<List<Color>> grid = null)
        {
            _rows = rows;
            _columns = columns;
            _pixelSize = pixelSize;

            _fillImage = null;

            _grid.Clear();

            if (grid != null)
            {
                foreach (var row in grid)
                {
                    _grid.Add(new List<Color>(row));
                }
            }
            else
            {
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
        }

        private Color _overlayColor = Color.FromArgb("#80000000");
        private int? _selectedRow = null;

        public async void Draw(ICanvas canvas, RectF dirtyRect)
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
                        canvas.FillColor = _grid[row][col];
                        canvas.FillRectangle(x, y, _pixelSize, _pixelSize);
                    }
                }
            }

            canvas.StrokeColor = Colors.Gray;
            canvas.StrokeSize = 1;

            for (int col = 0; col <= _columns; col++)
            {
                float x = col * _pixelSize;
                canvas.DrawLine(x, 0, x, _rows * _pixelSize);
            }


            for (int row = 0; row <= _rows; row++)
            {
                float y = row * _pixelSize;
                canvas.DrawLine(0, y, _columns * _pixelSize, y);
            }

            //if (_selectedRow.HasValue)
            //{
            //    float y = _selectedRow.Value * _pixelSize + _pixelSize / 2;
            //    canvas.StrokeColor = _overlayColor;
            //    canvas.StrokeSize = 2; 
            //    canvas.DrawLine(0, y, _columns * _pixelSize, y);
            //}

            if (_selectedRow.HasValue)
            {
                float y = _selectedRow.Value * _pixelSize;
                float height = _pixelSize;
                float width = _columns * _pixelSize;

                canvas.StrokeColor = _overlayColor;
                canvas.StrokeSize = 5; 
                canvas.DrawRectangle(0, y, width, height);
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

        public void HighlightRow(int row)
        {
            if (row == 0)
            {
                _selectedRow = 0;
            }
            else
            {
                int newRow = _selectedRow.Value + row;
                _selectedRow = Math.Clamp(newRow, 0, _rows - 1);
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

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    float x = col * _pixelSize;
                    float y = row * _pixelSize;

                    if (_grid[row][col] != Colors.Transparent)
                    {
                        using var paint = new SKPaint
                        {
                            Color = ConvertToSKColor(_grid[row][col]),
                            Style = SKPaintStyle.Fill
                        };
                        canvas.DrawRect(x, y, _pixelSize, _pixelSize, paint);
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
            using var imageData = image.Encode(SKEncodedImageFormat.Png, 100);

            using var memoryStream = new MemoryStream();
            imageData.SaveTo(memoryStream);

            string json = SerializeGridWithMetadata();
            byte[] jsonBytes = Encoding.UTF8.GetBytes("\n<GRID>\n" + json + "\n</GRID>\n");
            memoryStream.Write(jsonBytes, 0, jsonBytes.Length);

            File.WriteAllBytes(filePath, memoryStream.ToArray());
        }

        public string SerializeGridWithMetadata()
        {
            var serializableGrid = _grid.Select(row => row.Select(color => MyToHex(color)).ToList()).ToList();

            var meta = new
            {
                name = "Loom",
                rows = _rows,
                columns = _columns,
                pixelSize = _pixelSize,
                grid = serializableGrid
            };

            return JsonSerializer.Serialize(meta);
        }

        public string MyToHex(Color color)
        {
            byte a = (byte)(color.Alpha * 255);
            byte r = (byte)(color.Red * 255);
            byte g = (byte)(color.Green * 255);
            byte b = (byte)(color.Blue * 255);

            return $"#{a:X2}{r:X2}{g:X2}{b:X2}";
        }
    }
}