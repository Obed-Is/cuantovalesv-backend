using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class FiltersRequestDto
    {
        public List<string> Filters { get; set; } = new List<string>();
    }
}
