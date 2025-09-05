// ColorRepository.cs (Исправленный, с поддержкой DI)

using HandfulOfBreads.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandfulOfBreads.Data
{
    public class ColorRepository
    {
        private readonly AppDbContext _context;

        // Конструктор для Dependency Injection
        public ColorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, List<ColorItemViewModel>>> GetAllPalettesAsync()
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