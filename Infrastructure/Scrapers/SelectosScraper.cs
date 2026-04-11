using Core.DTOs;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Infrastructure.Scrapers
{
    public class SelectosScraper : IScraperService
    {
        public string NameSite => "Super Selectos";

        public string UrlSite => "https://www.superselectos.com/";

        public string UrlSearch => "https://www.superselectos.com/products?keyword=";

        //la url se le asigna completa desde la extraccion de la web
        public string UrlProduct => "";

        private IBrowser _browser;


        public SelectosScraper(IBrowser browser)
        {
            _browser = browser;
        }

        public async Task<List<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var products = new List<ProductDto>();
            var page = await _browser.NewPageAsync();

            try
            {
                await page.RouteAsync("**/*", async route =>
                {
                    var req = route.Request;
                    string[] blocked = { "image", "font", "stylesheet", "media", "websocket" };
                    string[] blockedUrls = { "google-analytics", "facebook", "hotjar", "gtag" };
                    if (blocked.Contains(req.ResourceType) || blockedUrls.Any(u => req.Url.Contains(u)))
                        await route.AbortAsync();
                    else
                        await route.ContinueAsync();
                });

                await page.GotoAsync($"{this.UrlSearch}{searchTerm}", new PageGotoOptions()
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded,
                    Timeout = 15000
                });

                await page.WaitForSelectorAsync(".prod-box-inner", new PageWaitForSelectorOptions
                {
                    State = WaitForSelectorState.Attached,
                    Timeout = 10000,
                });

                var contentElement = await page.QuerySelectorAllAsync(".prod-box-inner");

                foreach (var item in contentElement)
                {
                    if (products.Count >= 4) break;

                    var urlAndNameElement = await item.QuerySelectorAsync(".prod-nombre>a");
                    string urlProduct = urlAndNameElement != null ? await urlAndNameElement.GetAttributeAsync("href") ?? "Url no encontrada" : "Url no encontrada";

                    string name = urlAndNameElement != null ? await urlAndNameElement.TextContentAsync() ?? "Producto sin nombre" : "Producto sin nombre";

                    if (!name.ToLower().Contains($"{searchTerm.ToLower()}")) continue;

                    var priceElement = await item.QuerySelectorAsync(".precio");
                    string priceText = priceElement != null ? await priceElement.TextContentAsync() ?? "0.00" : "0.00";
                    decimal.TryParse(priceText.Replace("$", ""), out decimal price);

                    var imgElement = await item.QuerySelectorAsync("img");
                    string urlImg = imgElement != null ? await imgElement.GetAttributeAsync("src") ?? "Url de la imagen no encontrada" : "Url de la imagen no encontrada";

                    products.Add(new ProductDto()
                    {
                        Name = name,
                        NameSite = this.NameSite,
                        Price = price,
                        Url = (urlAndNameElement is not null) ? urlProduct : urlProduct,
                        UrlImg = urlImg,
                        UrlSite = this.UrlSite
                    });
                }

            }
            // los errores se capturan pero no detienen la ejecucion de los demas scrapers se manda el list solo
            catch (PlaywrightException plEx)
            {
                Console.WriteLine($"Error de playwright en el scraper de selectos: {plEx.Message}");
                //throw new AppExceptionStatusCode(500, "El motor de busqueda no respondio correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el scraper de selectos: {ex.Message}");
                //throw new AppExceptionStatusCode(500, "Error interno al procesar los datos");
            }
            finally
            {
                await page.CloseAsync();
            }

            sw.Stop();
            Console.WriteLine($"======== TIEMPO DE EJECUCION DE SELECTOS SCRAPER: {sw.ElapsedMilliseconds} ms ========");

            return products;
        }
    }
}
