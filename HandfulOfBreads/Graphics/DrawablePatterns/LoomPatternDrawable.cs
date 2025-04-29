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

        //canvas flags
        private Color _overlayColor = Color.FromArgb("#80000000");
        private int? _selectedRow = null;

        private (int Row, int Col)? _selectionStartCell;
        private (int Row, int Col)? _selectionEndCell;
        private bool _isSelecting = false;

        private List<(int Row, int Col)> SelectedCells = new();
        private List<List<Color>>? _pastingData;
        private int _pasteRows;
        private int _pasteCols;
        private (int Row, int Col)? _pastePosition;
        private bool _isPasting = false;

        private bool _isCutOperation = false;
        private List<(int Row, int Col, Color OriginalColor)> _backupBeforeCut = new();

        public bool IsPasting => _isPasting;

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

            //selection 
            if (_selectionStartCell.HasValue && _selectionEndCell.HasValue)
            {
                var start = _selectionStartCell.Value;
                var end = _selectionEndCell.Value;

                int minRow = Math.Min(start.Row, end.Row);
                int maxRow = Math.Max(start.Row, end.Row);
                int minCol = Math.Min(start.Col, end.Col);
                int maxCol = Math.Max(start.Col, end.Col);

                float x = minCol * _pixelSize;
                float y = minRow * _pixelSize;
                float width = (maxCol - minCol + 1) * _pixelSize;
                float height = (maxRow - minRow + 1) * _pixelSize;

                canvas.FillColor = new Color(152 / 255f, 105 / 255f, 77 / 255f, 0.2f);
                canvas.FillRectangle(x, y, width, height);

                canvas.StrokeColor = Color.FromArgb("#98694d");
                canvas.StrokeSize = 2;
                canvas.DrawRectangle(x, y, width, height);
            }

            // Draw clipboard if in pasting mode
            if (_isPasting && _pastingData != null && _pastePosition.HasValue)
            {
                int startRow = _pastePosition.Value.Row;
                int startCol = _pastePosition.Value.Col;

                for (int r = 0; r < _pasteRows; r++)
                {
                    for (int c = 0; c < _pasteCols; c++)
                    {
                        int gridRow = startRow + r;
                        int gridCol = startCol + c;

                        if (gridRow >= 0 && gridRow < _rows && gridCol >= 0 && gridCol < _columns)
                        {
                            var color = _pastingData[r][c];
                            if (color != Colors.Transparent)
                            {
                                float x = gridCol * _pixelSize;
                                float y = gridRow * _pixelSize;

                                // Напівпрозорий колір для попереднього перегляду
                                canvas.FillColor = color.WithAlpha(0.6f); // Зробимо трохи менш прозорим
                                canvas.FillRectangle(x, y, _pixelSize, _pixelSize);
                            }
                        }
                    }
                }

                // Додамо рамку навколо попереднього перегляду, щоб було видно межі
                float pasteX = startCol * _pixelSize;
                float pasteY = startRow * _pixelSize;
                float pasteWidth = _pasteCols * _pixelSize;
                float pasteHeight = _pasteRows * _pixelSize;

                canvas.StrokeColor = Colors.DodgerBlue; // Яскравий колір для рамки
                canvas.StrokeSize = 1;
                canvas.StrokeDashPattern = new float[] { 4, 2 }; // Пунктирна лінія
                canvas.DrawRectangle(pasteX, pasteY, pasteWidth, pasteHeight);
                canvas.StrokeDashPattern = null; // Скидаємо пунктир
            }

            //highlight row
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

        #region Selection
        public void UpdateSelectionCells((int Row, int Col)? start, (int Row, int Col)? end)
        {
            _selectionStartCell = start;
            _selectionEndCell = end;

            SelectedCells.Clear();

            if (start.HasValue && end.HasValue)
            {
                int minRow = Math.Min(start.Value.Row, end.Value.Row);
                int maxRow = Math.Max(start.Value.Row, end.Value.Row);
                int minCol = Math.Min(start.Value.Col, end.Value.Col);
                int maxCol = Math.Max(start.Value.Col, end.Value.Col);

                for (int row = minRow; row <= maxRow; row++)
                {
                    for (int col = minCol; col <= maxCol; col++)
                    {
                        if (row >= 0 && row < _rows && col >= 0 && col < _columns)
                        {
                            SelectedCells.Add((row, col));
                        }
                    }
                }
            }
        }

        public void CopySelection()
        {
            if (!SelectedCells.Any()) return;

            _pastingData = ExtractSelectedData();
            if (_pastingData == null || !_pastingData.Any() || !_pastingData[0].Any()) return; 

            _pasteRows = _pastingData.Count;
            _pasteCols = _pastingData[0].Count;
            _isCutOperation = false;
            _isPasting = true;

            int minRow = SelectedCells.Min(cell => cell.Row);
            int minCol = SelectedCells.Min(cell => cell.Col);

            _pastePosition = (minRow, minCol);

            ClearSelection();
        }

        public void CutSelection()
        {
            if (!SelectedCells.Any()) return; 

            _backupBeforeCut.Clear();
            foreach (var (row, col) in SelectedCells)
            {
                if (row >= 0 && row < _rows && col >= 0 && col < _columns)
                {
                    _backupBeforeCut.Add((row, col, _grid[row][col]));
                }
            }

            _pastingData = ExtractSelectedData();
            if (_pastingData == null || !_pastingData.Any() || !_pastingData[0].Any())
            {
                _backupBeforeCut.Clear();
                return;
            }

            foreach (var (row, col) in SelectedCells)
            {
                if (row >= 0 && row < _rows && col >= 0 && col < _columns)
                {
                    _grid[row][col] = Colors.Transparent;
                }
            }

            _pasteRows = _pastingData.Count;
            _pasteCols = _pastingData[0].Count;
            _isCutOperation = true;
            _isPasting = true;

            int minRow = SelectedCells.Min(cell => cell.Row);
            int minCol = SelectedCells.Min(cell => cell.Col);

            _pastePosition = (minRow, minCol);

            ClearSelection();
        }

        private void ClearSelection()
        {
            _selectionStartCell = null;
            _selectionEndCell = null;
            SelectedCells.Clear();
        }

        private List<List<Color>>? ExtractSelectedData()
        {
            if (!SelectedCells.Any()) return null;

            int minRow = SelectedCells.Min(c => c.Row);
            int maxRow = SelectedCells.Max(c => c.Row);
            int minCol = SelectedCells.Min(c => c.Col);
            int maxCol = SelectedCells.Max(c => c.Col);

            var data = new List<List<Color>>();

            for (int r = minRow; r <= maxRow; r++)
            {
                var rowData = new List<Color>();
                for (int c = minCol; c <= maxCol; c++)
                {
                    if (r >= 0 && r < _rows && c >= 0 && c < _columns)
                    {
                        rowData.Add(_grid[r][c]);
                    }
                    else
                    {
                        rowData.Add(Colors.Transparent);
                    }
                }
                data.Add(rowData);
            }
            return data;
        }

        private (int rowOffset, int colOffset) _dragOffset;

        public void SetPastePosition(int row, int col)
        {
            int newRow = row - _dragOffset.rowOffset;
            int newCol = col - _dragOffset.colOffset;

            int maxRow = _rows - _pasteRows;
            int maxCol = _columns - _pasteCols;

            int clampedRow = Math.Clamp(newRow, 0, Math.Max(0, maxRow));
            int clampedCol = Math.Clamp(newCol, 0, Math.Max(0, maxCol));

            _pastePosition = (clampedRow, clampedCol);
        }

        public void BeginPasteMove(int startTouchRow, int startTouchCol)
        {
            _dragOffset = (startTouchRow - _pastePosition.Value.Row, startTouchCol - _pastePosition.Value.Col);
        }

        public void ConfirmPaste()
        {
            if (!_isPasting || !_pastePosition.HasValue || _pastingData == null) return;

            int startRow = _pastePosition.Value.Row;
            int startCol = _pastePosition.Value.Col;

            for (int r = 0; r < _pasteRows; r++)
            {
                for (int c = 0; c < _pasteCols; c++)
                {
                    int gridRow = startRow + r;
                    int gridCol = startCol + c;

                    if (gridRow >= 0 && gridRow < _rows && gridCol >= 0 && gridCol < _columns)
                    {
                        if (_pastingData[r][c] != Colors.Transparent)
                        {
                            _grid[gridRow][gridCol] = _pastingData[r][c];
                        }
                    }
                }
            }

            // Скидаємо стан вставки
            ResetPasteState();
            _backupBeforeCut.Clear(); // Очищаємо бекап після успішної вставки
        }

        public void CancelPaste()
        {
            if (_isCutOperation && _backupBeforeCut.Any())
            {
                foreach (var (row, col, originalColor) in _backupBeforeCut)
                {
                    if (row >= 0 && row < _rows && col >= 0 && col < _columns) 
                    {
                        _grid[row][col] = originalColor;
                    }
                }
            }

            ResetPasteState();
            _backupBeforeCut.Clear();
        }

        private void ResetPasteState()
        {
            _isPasting = false;
            _pastingData = null;
            _pastePosition = null;
            _pasteRows = 0;
            _pasteCols = 0;
            _isCutOperation = false;
        }

        #endregion

        #region Everything else
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

        #endregion
    }
}