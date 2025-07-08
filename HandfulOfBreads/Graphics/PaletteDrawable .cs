using HandfulOfBreads.ViewModels;
using SkiaSharp;
using System.Text;

namespace HandfulOfBreads.Graphics
{
    public class PaletteDrawable
    {
        private readonly SKBitmap _bitmap;

        public int Columns { get; }
        public int CellSize { get; }
        public List<ColorItemViewModel> Colors { get; }

        public int Width => _bitmap.Width;
        public int Height => _bitmap.Height;

        public PaletteDrawable(PaletteBitmap bitmap)
        {
            using var ms = new MemoryStream(bitmap.GetBytes());
            _bitmap = SKBitmap.Decode(ms)!;

            Columns = bitmap.Columns;
            CellSize = bitmap.CellSize;
            Colors = bitmap.Colors;
        }

        public void Draw(SKCanvas canvas)
        {
            canvas.Clear(SKColors.White);

            canvas.DrawBitmap(_bitmap, 0, 0);

            int margin = 5;
            int cornerRadius = 5;

            for (int i = 0; i < Colors.Count; i++)
            {
                var colorVM = Colors[i];
                if (!colorVM.IsSelected)
                    continue;

                int col = i % Columns;
                int row = i / Columns;

                float left = margin + col * (CellSize + margin);
                float top = margin + row * (CellSize + margin);
                float right = left + CellSize;
                float bottom = top + CellSize;

                var rect = new SKRect(left, top, right, bottom);

                var borderPaint = new SKPaint
                {
                    Color = SKColors.Red,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 2,
                    IsAntialias = true
                };

                canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, borderPaint);
            }
        }
    }
}
