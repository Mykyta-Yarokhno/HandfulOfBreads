using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandfulOfBreads.Models
{
    public class Palette
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ColorItem> Colors { get; set; }
    }
}
