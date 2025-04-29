using SkiaSharp;
using System.Reflection;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace HandfulOfBreads.Graphics.DrawablePatterns
{
    internal class BrickPatternDrawable : IPatternDrawable
    {
        private readonly List<List<bool>> _grid = new();
        private int _rows;
        private int _columns;
        private int _pixelSize;

        private IImage? _fillImage;

        public Color SelectedColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsPasting => throw new NotImplementedException();

        public void InitializeGrid(int rows, int columns, int pixelSize, IImage? fillImage = null, List<List<Color>> grid = null)
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
            canvas.FillColor = Colors.OrangeRed;
            canvas.FillRectangle(dirtyRect);

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    float offsetY = (col % 2 != 0) ? _pixelSize / 2 : 0;

                    float x = col * _pixelSize;
                    float y = row * _pixelSize + offsetY;

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
                    else
                    {
                        canvas.FillColor = Colors.White;
                        canvas.FillRectangle(x, y, _pixelSize, _pixelSize);
                    }

                    canvas.StrokeColor = Colors.Gray;
                    canvas.StrokeSize = 1;
                    canvas.DrawRectangle(x, y, _pixelSize, _pixelSize);
                }
            }
        }



        public void TogglePixel(float tapX, float tapY)
        {
            int col = (int)(tapX / _pixelSize);

            float offsetY = (col % 2 != 0) ? _pixelSize / 2 : 0;

            int row = (int)((tapY - offsetY) / _pixelSize);

            if (row >= 0 && col >= 0 && col < _columns)
            {
                if (row == 0 && col % 2 != 0)
                {
                    float localTapY = tapY - (row * _pixelSize + offsetY);
                    if (localTapY < 0)
                    {
                        return;
                    }
                }

                if (row < _rows)
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
            int width = _columns * _pixelSize + 20;
            int height = _rows * _pixelSize + 20;

            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            var resourcePath = "HandfulOfBreads.Resources.Images.bonk.png";
            var _fillImage2 = LoadBitmapFromResource(resourcePath);

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    float offsetY = (col % 2 != 0) ? _pixelSize / 2 : 0;

                    float x = col * _pixelSize;
                    float y = row * _pixelSize + offsetY;

                    if (_grid[row][col])
                    {
                        if (_fillImage2 != null)
                        {
                            var rect = new SKRect(x, y, x + _pixelSize, y + _pixelSize);
                            canvas.DrawBitmap(_fillImage2, rect);
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
                    else
                    {
                        using var paint = new SKPaint
                        {
                            Color = SKColors.White,
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
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            using (var stream = File.OpenWrite(filePath))
            {
                data.SaveTo(stream);
            }
        }

        public void HighlightRow(int row)
        {
            throw new NotImplementedException();
        }

        public void UpdateSelection(PointF? start, PointF? end)
        {
            throw new NotImplementedException();
        }

        public void UpdateSelectionCells((int Row, int Col)? start, (int Row, int Col)? end)
        {
            throw new NotImplementedException();
        }

        public void CopySelection()
        {
            throw new NotImplementedException();
        }

        public void CutSelection()
        {
            throw new NotImplementedException();
        }

        public void ConfirmPaste()
        {
            throw new NotImplementedException();
        }

        public void CancelPaste()
        {
            throw new NotImplementedException();
        }

        public void SetPastePosition(int row, int col)
        {
            throw new NotImplementedException();
        }

        public void BeginPasteMove(int startTouchRow, int startTouchCol)
        {
            throw new NotImplementedException();
        }

        public void HighlightRow(int? row)
        {
            throw new NotImplementedException();
        }
    }
}

