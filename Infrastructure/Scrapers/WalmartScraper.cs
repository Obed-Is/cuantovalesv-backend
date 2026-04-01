using Core.DTOs;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.WebRequestMethods;
using Microsoft.Playwright;
using Infrastructure.Services;

namespace Infrastructure.Scrapers
{
    public class WalmartScraper : IScraperService
    {
        public string NameSite => "Walmart";

        public string UrlSite => "https://www.walmart.com.sv/";

        public string UrlSearch => "https://www.walmart.com.sv/";

        public string UrlProduct => "https://www.walmart.com.sv/";

        private readonly IBrowser _browser;

        public WalmartScraper(IBrowser browser)
        {
            _browser = browser;
        }

        public async Task<List<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            var products = new List<ProductDto>();
            var page = await _browser.NewPageAsync();

            try
            {
                await page.GotoAsync($"{UrlSearch}{searchTerm}");
                var resultContent = await page.QuerySelectorAllAsync("[data-af-product-position]");
                foreach (var item in resultContent)
                {
                    if (products.Count >= 4) break;

                    var urlElement = await item.QuerySelectorAsync("a.vtex-product-summary-2-x-clearLink");
                    string urlProduct = urlElement != null ? await urlElement.GetAttributeAsync("href") ?? "Url no encontrada" : "Url no encontrada";

                    var nameElement = await item.QuerySelectorAsync("span#product-summary-sku-name");
                    string name = nameElement != null ? await nameElement.InnerTextAsync() : "Producto sin nombre";

                    var priceElement = await item.QuerySelectorAsync(".price-container>div>div>span>span>span");
                    var priceText = priceElement != null ? await priceElement.InnerTextAsync() : "0.00";
                    decimal.TryParse(priceText?.Replace("$", "").Trim(), out decimal price);

                    var imgElement  = await item.QuerySelectorAsync("img.vtex-product-summary-2-x-imageNormal");
                    string imgUrl = imgElement != null ? await imgElement.GetAttributeAsync("src") ?? "Url de la imagen no encontrada" : "Url de la imagen no encontrada";

                    products.Add(new ProductDto()
                    {
                        Name = name,
                        NameSite = this.NameSite,
                        Price = price,
                        Url = this.UrlSite + urlProduct,
                        UrlImg = imgUrl,
                        UrlSite = this.UrlSite
                    });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await page.CloseAsync();
            }

            return products;
        }
    }
}
