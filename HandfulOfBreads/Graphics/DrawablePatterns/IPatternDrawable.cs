using IImage = Microsoft.Maui.Graphics.IImage;

namespace HandfulOfBreads.Graphics.DrawablePatterns
{
    public interface IPatternDrawable : IDrawable
    {
        Color SelectedColor { get; set; }
        bool IsPasting { get;}

        void InitializeGrid(int rows, int columns, int pixelSize, IImage? fillImage = null, List<List<Color>> grid = null);
        void Draw(ICanvas canvas, RectF dirtyRect);
        void TogglePixel(float x, float y);
        Task SaveToFileAsync(string filePath);

        public void HighlightRow(int? row);
        public void UpdateSelectionCells((int Row, int Col)? start, (int Row, int Col)? end);
        public void CopySelection();
        public void CutSelection();
        public void ConfirmPaste();
        public void CancelPaste();
        public void SetPastePosition(int row, int col);
        public void BeginPasteMove(int startTouchRow, int startTouchCol);
    }
}
