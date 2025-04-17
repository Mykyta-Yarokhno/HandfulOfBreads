using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HandfulOfBreads.Services
{
    public class GridLoadingService
    {
        public async Task<(string name, int rows, int columns, int pixelSize, List<List<Color>> grid)> LoadGridFromFileAsync(string filePath)
        {
            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
            string fileText = Encoding.UTF8.GetString(fileBytes);

            int gridStart = fileText.IndexOf("<GRID>");
            int gridEnd = fileText.IndexOf("</GRID>");

            if (gridStart == -1 || gridEnd == -1)
                throw new Exception("Не знайдено JSON-метадані у файлі.");

            string json = fileText.Substring(gridStart + "<GRID>".Length, gridEnd - gridStart - "<GRID>".Length).Trim();
            var meta = JsonSerializer.Deserialize<GridMeta>(json);

            if (meta is null)
                throw new Exception("Не вдалося розпарсити JSON.");

            var grid = meta.grid
                .Select(row => row.Select(hex => FromHex(hex)).ToList())
                .ToList();

            return (meta.name, meta.rows, meta.columns, meta.pixelSize, grid);
        }

        private Color FromHex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return Colors.Transparent;
            if (hex.StartsWith("#")) hex = hex[1..];

            if (hex.Length == 6)
            {
                byte r = Convert.ToByte(hex[..2], 16);
                byte g = Convert.ToByte(hex[2..4], 16);
                byte b = Convert.ToByte(hex[4..6], 16);
                return Color.FromRgb(r, g, b);
            }
            else if (hex.Length == 8)
            {
                byte a = Convert.ToByte(hex[..2], 16);
                byte r = Convert.ToByte(hex[2..4], 16);
                byte g = Convert.ToByte(hex[4..6], 16);
                byte b = Convert.ToByte(hex[6..8], 16);
                return Color.FromRgba(r, g, b, a);
            }

            return Colors.Transparent;
        }

        public class GridMeta
        {
            public string name { get; set; }
            public int rows { get; set; }
            public int columns { get; set; }
            public int pixelSize { get; set; }
            public List<List<string>> grid { get; set; }
        }
    }
}
