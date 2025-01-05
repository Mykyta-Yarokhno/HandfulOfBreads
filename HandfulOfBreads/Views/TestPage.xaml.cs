using CommunityToolkit.Maui.Core;

namespace HandfulOfBreads.Views;
public partial class TestPage : ContentPage
{
    private readonly PixelGridDrawable _drawable = new();
    private const int PixelSize = 20; // Размер одной клетки
    private bool _isDrawing;

    public TestPage()
    {
        InitializeComponent();
        Drawable = _drawable;
        BindingContext = this;

        // Инициализируем сетку размером 32x32
        _drawable.InitializeGrid(32, 32, PixelSize);
    }

    public IDrawable Drawable { get; }

    private void OnStartInteraction(object sender, TouchEventArgs e)
    {
        _isDrawing = true;
        HandleInteraction(e.Touches[0]);
    }

    private void OnDragInteraction(object sender, TouchEventArgs e)
    {
        if (_isDrawing)
        {
            HandleInteraction(e.Touches[0]);
        }
    }

    private void HandleInteraction(PointF touchPoint)
    {
        _drawable.TogglePixel(touchPoint.X, touchPoint.Y);
        PixelGraphicsView.Invalidate(); // Обновить отображение
    }
}

public class PixelGridDrawable : IDrawable
{
    private readonly List<List<bool>> _grid = new();
    private int _rows;
    private int _columns;
    private int _pixelSize;

    public void InitializeGrid(int rows, int columns, int pixelSize)
    {
        _rows = rows;
        _columns = columns;
        _pixelSize = pixelSize;

        // Инициализация сетки (false — пиксель не закрашен)
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
        canvas.FillRectangle(dirtyRect); // Очистка экрана

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                float x = col * _pixelSize;
                float y = row * _pixelSize;

                // Закрашенный пиксель
                if (_grid[row][col])
                {
                    canvas.FillColor = Colors.Black;
                    canvas.FillRectangle(x, y, _pixelSize, _pixelSize);
                }

                // Границы пикселей
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
            if (!_grid[row][col]) // Меняем цвет только если пиксель черный
            {
                _grid[row][col] = true;
            }
        }
    }   
}

