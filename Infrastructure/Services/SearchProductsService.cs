using Core.DTOs;
using Core.Exceptions;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class SearchProductsService : ISearchService
    {
        private readonly IEnumerable<IScraperService> _scraperServices;

        public SearchProductsService(IEnumerable<IScraperService> scrapers)
        {
            _scraperServices = scrapers;
        }

        public async Task<List<ProductDto>> SearchAll(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm.Trim()) || searchTerm.Length >= 40) throw new AppExceptionStatusCode(400, "Los datos ingresados son invalidos");

            var products = new List<ProductDto>();

            foreach(var scraper in _scraperServices)
            {
                var product = await scraper.SearchProductsAsync(searchTerm);
                products.AddRange(product);
            }

            return products;
        }
    }
}
