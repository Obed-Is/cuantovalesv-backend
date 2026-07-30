using HtmlAgilityPack;
using Scrapers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Scrapers.SitesWeb
{
    public class WalmartScraper
    {
        //configuracion para peticiones httpss
        private readonly HttpClient _httpClient;
        private HttpClientHandler handler = new HttpClientHandler
        {
            AutomaticDecompression =
              DecompressionMethods.GZip |
              DecompressionMethods.Deflate |
              DecompressionMethods.Brotli
        };
        private readonly string _sitioWeb = "https://www.walmart.com.sv/";

        public WalmartScraper()
        {
            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Add(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/137.0.0.0 Safari/537.36"
            );
        }
        

        public async Task<List<ProductoModel>> ObtenerUnProducto(string productoBuscar)
        {
            List<ProductoModel> productosEncontrados = new();

            string urlBuscar = string.Concat(_sitioWeb, productoBuscar);
            var response = await _httpClient.GetAsync(urlBuscar);

            var content = await response.Content.ReadAsStringAsync();

            var docs = new HtmlDocument();
            docs.LoadHtml(content);
            var productos = docs.DocumentNode.SelectNodes(
                $"//div[@data-af-element='search-result']"
            );

            foreach (var producto in productos)
            {
                var imgHtml = producto.SelectSingleNode(".//img");
                var urlImg = imgHtml?.GetAttributeValue("src", _sitioWeb) ?? "";
                var precioProducto = producto.SelectSingleNode(".//div[@class='price-container']")?.InnerText.Trim() ?? "No se pudo obtener el precio o el producto esta agotado";
                var linkHtml = producto.SelectSingleNode(".//a[@class='vtex-product-summary-2-x-clearLink h-100 flex flex-column']")?.GetAttributeValue("href", _sitioWeb) ?? "";

                precioProducto = (precioProducto.Contains("$")) ? string.Concat("$", precioProducto?.Split('$')[1]) : precioProducto;

                productosEncontrados.Add(new ProductoModel()
                {
                    Nombre = producto.SelectSingleNode(".//span[@id='product-summary-sku-name']")?.InnerText.Trim() ?? $"Producto relacionado a {productoBuscar}",
                    NombreSitio = "Walmart",
                    Precio = precioProducto,
                    UrlImagen = urlImg,
                    UrlProducto = string.Concat(_sitioWeb, linkHtml),
                    UrlSitio = _sitioWeb
                });
            }

            return productosEncontrados.DistinctBy(prd => prd.UrlProducto).ToList();
        }
    }
}
