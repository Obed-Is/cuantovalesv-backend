using Scrapers.Models;
using Scrapers.SitesWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrapers.Services
{
    public class ScraperService
    {
        private readonly WalmartScraper _walmartScraper;
        public ScraperService(WalmartScraper walmartScraper)
        {
            _walmartScraper = walmartScraper;
        }

        public async Task<List<ProductoModel>> ObtenerProductosAsync(string productoBuscar)
        {
            Console.WriteLine("Obteniendo productos...");
            List<ProductoModel> productos = await _walmartScraper.ObtenerUnProducto(productoBuscar);

            return productos;
        }
    }
}
