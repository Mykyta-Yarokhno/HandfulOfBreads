using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Maui.Controls;
using HandfulOfBreads.ViewModels;

public static class ColorPaletteBitmapCache
{
    private static readonly Dictionary<string, PaletteBitmap> _paletteBitmaps = new();

    private static Dictionary<string, List<ColorItemViewModel>> _allPalettes = new();

    public static void InitializeAllPalettes(Dictionary<string, List<ColorItemViewModel>> allPalettes)
    {
        _allPalettes = allPalettes;

        foreach (var kvp in allPalettes)
        {
            _paletteBitmaps[kvp.Key] = GeneratePaletteBitmap(kvp.Value);
        }
    }

    public static PaletteBitmap? GetPaletteBitmap(string paletteName)
    {
        return _paletteBitmaps.TryGetValue(paletteName, out var bitmap) ? bitmap : null;
    }

    public static List<ColorItemViewModel>? GetPaletteColors(string paletteName)
    {
        return _allPalettes.TryGetValue(paletteName, out var colors) ? colors : null;
    }

    public static PaletteBitmap GeneratePaletteBitmap(
            List<ColorItemViewModel> colors,
            int columns = 5,
            int totalWidth = 300,
            int margin = 5,
            int cornerRadius = 5)
    {
        int cellSize = (totalWidth - (columns + 1) * margin) / columns;
        int rows = (int)Math.Ceiling(colors.Count / (double)columns);
        int totalHeight = rows * cellSize + (rows + 1) * margin;

        using var bitmap = new SKBitmap(totalWidth, totalHeight);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.White);

        var fontSize = 10;

        foreach (var (colorVM, i) in colors.Select((c, i) => (c, i)))
        {
            int col = i % columns;
            int row = i / columns;

            float left = margin + col * (cellSize + margin);
            float top = margin + row * (cellSize + margin);
            float right = left + cellSize;
            float bottom = top + cellSize;

            var rect = new SKRect(left, top, right, bottom);

            // Тінь (імітація)
            //if (true)
            //{
            //    var shadowPaint = new SKPaint
            //    {
            //        Color = new SKColor(0, 0, 0, 50),
            //        IsAntialias = true
            //    };
            //    var shadowRect = new SKRect(rect.Left + 2, rect.Top + 2, rect.Right + 2, rect.Bottom + 2);
            //    canvas.DrawRoundRect(shadowRect, cornerRadius, cornerRadius, shadowPaint);
            //}

            // Кольоровий фон з заокругленням
            var fillPaint = new SKPaint
            {
                Color = SKColor.Parse(colorVM.HexColor),
                IsAntialias = true
            };
            canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, fillPaint);          

            var borderPaint = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2,
                IsAntialias = true
            };

            canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, borderPaint);
       
            // Центральний Code
            if (!string.IsNullOrWhiteSpace(colorVM.Code))
            {
                var codePaint = new SKPaint
                {
                    Color = SKColors.Black,
                    TextSize = fontSize,
                    IsAntialias = true,
                    TextAlign = SKTextAlign.Center
                };
                float x = rect.MidX;
                float y = rect.MidY + fontSize / 2;
                canvas.DrawText(colorVM.Code, x, y, codePaint);
            }

            // Sign по кутках
            if (colorVM.Sign.HasValue)
            {
                var sign = colorVM.Sign.Value.ToString();
                var signPaint = new SKPaint
                {
                    Color = SKColors.Black,
                    TextSize = fontSize,
                    IsAntialias = true
                };

                float sMargin = 3;

                // Top-left
                canvas.DrawText(sign, rect.Left + sMargin, rect.Top + fontSize + sMargin, signPaint);

                // Top-right
                float trWidth = signPaint.MeasureText(sign);
                canvas.DrawText(sign, rect.Right - trWidth - sMargin, rect.Top + fontSize + sMargin, signPaint);

                // Bottom-left
                canvas.DrawText(sign, rect.Left + sMargin, rect.Bottom - sMargin, signPaint);

                // Bottom-right
                float brWidth = signPaint.MeasureText(sign);
                canvas.DrawText(sign, rect.Right - brWidth - sMargin, rect.Bottom - sMargin, signPaint);
            }
        }

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        var bytes = data.ToArray();

        return new PaletteBitmap(bytes, colors, columns, cellSize);
    }
}

public class PaletteBitmap
{
    private readonly byte[] _bytes;

    public List<ColorItemViewModel> Colors { get; }
    public int Columns { get; }
    public int CellSize { get; }

    public PaletteBitmap(byte[] bytes, List<ColorItemViewModel> colors, int columns, int cellSize)
    {
        _bytes = bytes;
        Colors = colors;
        Columns = columns;
        CellSize = cellSize;
    }

    public byte[] GetBytes() => _bytes;

    public ImageSource ToImageSource()
        => ImageSource.FromStream(() => new MemoryStream(_bytes));
}
