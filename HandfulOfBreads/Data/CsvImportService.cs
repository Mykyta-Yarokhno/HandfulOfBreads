using CsvHelper;
using HandfulOfBreads.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;

public class CsvImportService
{
    private readonly AppDbContext _context;

    public CsvImportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task ImportAllPalettesAsync()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith("HandfulOfBreads.Resources.Raw.ColorPalettes.") && name.EndsWith(".csv"))
            .ToList();

        var allPalettes = _context.Palettes
        .Include(p => p.Colors)
        .ToList();

        var paletteDict = allPalettes.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var resourceName in resourceNames)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<ColorCsvDto>().ToList();

            foreach (var record in records)
            {
                if (!paletteDict.TryGetValue(record.PaletteName, out var palette))
                {
                    palette = new Palette
                    {
                        Name = record.PaletteName,
                        Colors = new List<ColorItem>()
                    };

                    _context.Palettes.Add(palette);
                    paletteDict[record.PaletteName] = palette;
                }

                bool exists = palette.Colors.Any(c =>
                    c.Code == record.Code &&
                    c.HexColor == record.HexColor &&
                    c.Type == record.Type &&
                    c.Sign == record.Sign);

                if (!exists)
                {
                    palette.Colors.Add(new ColorItem
                    {
                        Code = record.Code,
                        HexColor = record.HexColor,
                        Type = record.Type,
                        Sign = record.Sign
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
    }
}

public class ColorCsvDto
{
    public string Code { get; set; }
    public string HexColor { get; set; }
    public string Type { get; set; }
    public char? Sign { get; set; }
    public string PaletteName { get; set; }
}
