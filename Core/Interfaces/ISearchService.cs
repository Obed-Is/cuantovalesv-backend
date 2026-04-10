using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface ISearchService
    {
        Task<List<ProductDto>> SearchAll(string searchTermn);

        Task<List<ProductDto>> SearchProductsFilter(FiltersRequestDto filters, string searchTermn);
    }
}
