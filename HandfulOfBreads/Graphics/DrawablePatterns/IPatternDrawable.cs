using IImage = Microsoft.Maui.Graphics.IImage;

namespace HandfulOfBreads.Graphics.DrawablePatterns
{
    internal interface IPatternDrawable : IDrawable
    {
        void InitializeGrid(int rows, int columns, int pixelSize, IImage? fillImage = null);
        void Draw(ICanvas canvas, RectF dirtyRect);
        void TogglePixel(float x, float y);
        Task SaveToFileAsync(string filePath);
    }
}
