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

namespace Infrastructure.Scrapers
{
    public class SimanScraper : IScraperService
    {
        public string NameSite => "Siman";

        public string UrlSite => "https://sv.siman.com/";

        public string UrlSearch => "https://sv.siman.com/search?_q=";

        public string UrlProduct => "https://sv.siman.com/";

        private IBrowser _browser;

        public SimanScraper(IBrowser browser)
        {
            _browser = browser;
        }

        public async Task<List<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<ProductDto> products = new List<ProductDto>();
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

                await page.GotoAsync($"{UrlSearch}{searchTerm}", new PageGotoOptions()
                {
                    WaitUntil = WaitUntilState.DOMContentLoaded,
                    Timeout = 15000
                });

                await page.WaitForFunctionAsync("document.querySelectorAll('.siman-algolia-react-2-x-hitLinkItem').length >= 2", new PageWaitForFunctionOptions()
                {
                    Timeout = 10000
                });

                var contentElement = await page.QuerySelectorAllAsync(".siman-algolia-react-2-x-hitItem");

                foreach (var item in contentElement)
                {
                    if (products.Count >= 4) break;

                    var urlElement = await item.QuerySelectorAsync("a");
                    string urlProduct = urlElement != null ? await urlElement.GetAttributeAsync("href") ?? "Url no encontrada" : "Url no encontrada";


                    var nameElement = await item.QuerySelectorAsync(".siman-algolia-react-2-x-searchProductsItemName");
                    string name = nameElement != null ? await nameElement.InnerTextAsync() ?? "Producto sin nombre" : "Producto sin nombre";

                    var priceElement = await item.QuerySelectorAsync(".siman-algolia-react-2-x-price");
                    string priceText = priceElement != null ? await priceElement.InnerTextAsync() ?? "0.00" : "0.00";
                    decimal.TryParse(priceText.Replace("$", ""), out decimal price);

                    var imgElement = await item.QuerySelectorAsync(".siman-algolia-react-2-x-product-image__main");
                    string urlImg = imgElement != null ? await imgElement.GetAttributeAsync("src") ?? "Url de la imagen no encontrada" : "Url de la imagen no encontrada";

                    products.Add(new ProductDto()
                    {
                        Name = name,
                        NameSite = this.NameSite,
                        Price = price,
                        Url = (urlElement is not null) ? this.UrlSite + urlProduct : urlProduct,
                        UrlImg = urlImg,
                        UrlSite = this.UrlSite
                    });
                }
            }
            // los errores se capturan pero no detienen la ejecucion de los demas scrapers se manda el list solo
            catch (PlaywrightException plEx)
            {
                Console.WriteLine($"Error de playwright en el scraper de siman: {plEx.Message}");
                //throw new AppExceptionStatusCode(500, "El motor de busqueda no respondio correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el scraper de siman: {ex.Message}");
                //throw new AppExceptionStatusCode(500, "Error interno al procesar los datos");
            }
            finally
            {
                await page.CloseAsync();
            }
            sw.Stop();
            Console.WriteLine($"======== TIEMPO DE EJECUCION DE SIMAN SCRAPER: {sw.ElapsedMilliseconds} ms ========");
            return products;
        }
    }
}
