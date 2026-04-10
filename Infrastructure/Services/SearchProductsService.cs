using Core.DTOs;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Scrapers;
using System;
using System.Collections;
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

            //ejecuta todos los scrapers en paralelo(los crea tipo Task)
            var tasks = _scraperServices.Select(async sc => await sc.SearchProductsAsync(searchTerm)).ToList();
            // espera que todos los scrapers finalizen
            List<ProductDto>[] results = await Task.WhenAll(tasks);
            // convierte los restultados a 1 nivel ejm: [1, 2, [3, 4], [5, 6,], 7] = [1, 2, 3, 4, 5, 6, 7] 
            var products = results.SelectMany(p => p).ToList();

            return products;
        }

        public async Task<List<ProductDto>> SearchProductsFilter(FiltersRequestDto filters, string searchTerm)
        {
            if (filters.Filters.Count <= 0 || string.IsNullOrWhiteSpace(searchTerm.Trim()) || searchTerm.Length >= 40) throw new AppExceptionStatusCode(400, "Los datos ingresados son invalidos");
            var products = new List<ProductDto>();

            List<string> filtersParse = filters.Filters.Select(s => s.ToLower()).ToList();
            var scrapersList = new List<IScraperService>();
            foreach (var item in _scraperServices)
            {
                if (filtersParse.Contains(item.NameSite.ToLower()))
                {
                    scrapersList.Add(item);
                }
            }

            if (scrapersList.Count <= 0) throw new AppExceptionStatusCode(404, "Los filtros indicados son invalidos");
            var task = scrapersList.Select(async sc => await sc.SearchProductsAsync(searchTerm)).ToList();
            List<ProductDto>[] results = await Task.WhenAll(task);
            products = [.. results.SelectMany(pd => pd)];

            return products;
        }
    }
}
