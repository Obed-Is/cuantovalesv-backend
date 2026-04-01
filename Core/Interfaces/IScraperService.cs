using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface IScraperService
    {
        string NameSite { get; }
        string UrlSite { get; }
        string UrlSearch { get; }
        string UrlProduct { get; }
        Task<List<ProductDto>> SearchProductsAsync(string searchTerm);
    }
}
