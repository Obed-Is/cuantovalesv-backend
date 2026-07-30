using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapers.Models
{
    public class ProductoModel
    {
        public string Nombre { get; set; } = string.Empty;
        public string Precio { get; set; } = string.Empty;
        public string NombreSitio { get; set; } = string.Empty;
        public string UrlProducto { get; set; } = string.Empty;
        public string UrlImagen { get; set; } = string.Empty;
        public string UrlSitio { get; set; } = string.Empty;
    }
}
