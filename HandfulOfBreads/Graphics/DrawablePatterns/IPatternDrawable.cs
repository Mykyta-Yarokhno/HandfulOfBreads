using IImage = Microsoft.Maui.Graphics.IImage;

namespace HandfulOfBreads.Graphics.DrawablePatterns
{
    internal interface IPatternDrawable : IDrawable
    {
        Color SelectedColor { get; set; }
        void InitializeGrid(int rows, int columns, int pixelSize, IImage? fillImage = null, List<List<Color>> grid = null);
        void Draw(ICanvas canvas, RectF dirtyRect);
        void TogglePixel(float x, float y);
        Task SaveToFileAsync(string filePath);

        public void HighlightRow(int row);
    }
}
