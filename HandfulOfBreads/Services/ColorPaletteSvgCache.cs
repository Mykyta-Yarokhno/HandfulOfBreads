using HandfulOfBreads.Constants;
using HandfulOfBreads.Models;
using HandfulOfBreads.ViewModels;
using SkiaSharp;
using Svg.Skia;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SKSvg = Svg.Skia.SKSvg;

public class ColorPaletteSvgCache
{
    private static readonly Dictionary<string, string> _paletteSvgStrings = new();
    private static readonly Dictionary<string, SKSvg> _paletteSvgs = new();
    private static Dictionary<string, List<ColorItemViewModel>> _allPalettes = new();
    private static List<ColorItemViewModel> _usedColors = new();

    public  void InitializeAllPalettes(Dictionary<string, List<ColorItemViewModel>> allPalettes)
    {
        _allPalettes = allPalettes;
        foreach (var kvp in _allPalettes)
        {
            _paletteSvgStrings[kvp.Key] = GeneratePaletteSvgString(kvp.Value);
            _paletteSvgs[kvp.Key] = GeneratePaletteSvg(kvp.Value, totalWidth: 300);
        }
        _allPalettes[PaletteNames.UsedColors] = _usedColors;
        _paletteSvgStrings[PaletteNames.UsedColors] = GeneratePaletteSvgString(_usedColors);
    }

    public static List<ColorItem> GetAllPalettesColors()
    {
        return _allPalettes
            .SelectMany(p => p.Value)
            .GroupBy(c => c.Code, StringComparer.OrdinalIgnoreCase)
            .Select(g => new ColorItem
            {
                Code = g.Key,
                HexColor = g.First().HexColor,
                Sign = g.First().Sign
            })
            .ToList();
    }

    public static void UpdateUsedColors(IEnumerable<ColorItemViewModel> usedColors)
    {
        _usedColors = usedColors
            .GroupBy(c => c.Code, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();
        _allPalettes[PaletteNames.UsedColors] = _usedColors;
        _paletteSvgStrings[PaletteNames.UsedColors] = GeneratePaletteSvgString(_usedColors);
    }

    public static SKSvg? GetPaletteSvg(string paletteName)
    {
        return _paletteSvgs.TryGetValue(paletteName, out var svg) ? svg : null;
    }

    public static SKSvg GeneratePaletteSvg(
        List<ColorItemViewModel> colors,
        int columns = 5,
        int totalWidth = 300,
        int margin = 5,
        int cornerRadius = 5)
    {
        var svgString = GeneratePaletteSvgString(colors, columns, totalWidth, margin, cornerRadius);
        var svg = new SKSvg();
        svg.Load(new MemoryStream(Encoding.UTF8.GetBytes(svgString)));
        return svg;
    }

    public static List<ColorItemViewModel>? GetPaletteColors(string paletteName)
    {
        if (paletteName == "Used Сolours")
            return _usedColors ?? new List<ColorItemViewModel>();

        return _allPalettes.TryGetValue(paletteName, out var colors) ? colors : null;
    }

    private static string GeneratePaletteSvgString(
        List<ColorItemViewModel> colors,
        int columns = 5,
        int totalWidth = 300,
        int margin = 5,
        int cornerRadius = 5)
    {
        int cellSize = (totalWidth - (columns + 1) * margin) / columns;
        int rows = (int)Math.Ceiling(colors.Count / (double)columns);
        int totalHeight = rows * cellSize + (rows + 1) * margin;

        var svgBuilder = new StringBuilder();
        svgBuilder.AppendLine($"<svg width=\"{totalWidth}\" height=\"{totalHeight}\" xmlns=\"http://www.w3.org/2000/svg\">");

        var fontSize = 10;
        var signFontSize = 10;

        foreach (var (colorVM, i) in colors.Select((c, i) => (c, i)))
        {
            int col = i % columns;
            int row = i / columns;

            float left = margin + col * (cellSize + margin);
            float top = margin + row * (cellSize + margin);

            svgBuilder.AppendLine($"  <rect x=\"{left}\" y=\"{top}\" width=\"{cellSize}\" height=\"{cellSize}\" rx=\"{cornerRadius}\" ry=\"{cornerRadius}\" fill=\"{colorVM.HexColor}\" stroke=\"#000000\" stroke-width=\"1\"/>");

            if (!string.IsNullOrWhiteSpace(colorVM.Code))
            {
                float textX = left + cellSize / 2;
                float textY = top + cellSize / 2;
                svgBuilder.AppendLine($"  <text x=\"{textX - 18.5}\" y=\"{textY + 3.5}\" font-size=\"{fontSize}\" text-anchor=\"start\" dominant-baseline=\"middle\" fill=\"#000000\">{colorVM.Code}</text>");
            }

            if (colorVM.Sign.HasValue)
            {
                var sign = colorVM.Sign.Value.ToString();
                float sMargin = 3;

                svgBuilder.AppendLine($"  <text x=\"{left + sMargin}\" y=\"{top + signFontSize + sMargin}\" font-size=\"{signFontSize}\" fill=\"#000000\">{sign}</text>");
                svgBuilder.AppendLine($"  <text x=\"{left + cellSize - sMargin}\" y=\"{top + signFontSize + sMargin}\" font-size=\"{signFontSize}\" text-anchor=\"end\" fill=\"#000000\">{sign}</text>");
                svgBuilder.AppendLine($"  <text x=\"{left + sMargin}\" y=\"{top + cellSize - sMargin}\" font-size=\"{signFontSize}\" dominant-baseline=\"alphabetic\" fill=\"#000000\">{sign}</text>");
                svgBuilder.AppendLine($"  <text x=\"{left + cellSize - sMargin}\" y=\"{top + cellSize - sMargin}\" font-size=\"{signFontSize}\" text-anchor=\"end\" dominant-baseline=\"alphabetic\" fill=\"#000000\">{sign}</text>");
            }
        }

        svgBuilder.AppendLine("</svg>");
        return svgBuilder.ToString();
    }
}