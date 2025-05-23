using HandfulOfBreads.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandfulOfBreads.Data
{
    public static class ColorRepository
    {
        private static AppDbContext _context;
        public static void Initialize(AppDbContext context)
        {
            _context = context;
        }

        public static async Task<Dictionary<string, List<ColorItemViewModel>>> GetAllPalettesAsync()
        {
            var palettes = await _context.Palettes.Include(p => p.Colors).ToListAsync();

            var result = new Dictionary<string, List<ColorItemViewModel>>();

            foreach (var palette in palettes)
            {
                var colorVMs = palette.Colors
                    .Select(c => new ColorItemViewModel(c)) 
                    .ToList();

                result[palette.Name] = colorVMs;
            }

            return result;
        }
    }

}
