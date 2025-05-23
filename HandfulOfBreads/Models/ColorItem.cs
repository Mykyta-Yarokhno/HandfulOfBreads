using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandfulOfBreads.Models
{
    public class ColorItem
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string HexColor { get; set; }
        public string? Type { get; set; }
        public char? Sign { get; set; }

        public int PaletteId { get; set; }
        public Palette Palette { get; set; }
    }
}
