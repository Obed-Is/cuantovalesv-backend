using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class SearchProductsController : ControllerBase
    {
        private readonly ISearchService _scraperService;

        public SearchProductsController(ISearchService scraperService)
        {
            _scraperService = scraperService;
        }

        [HttpGet]
        public async Task<ActionResult> GetProducts([FromQuery] string term)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var products = await _scraperService.SearchAll(term);
            sw.Stop();
            Console.WriteLine($"|========| DURACION DE PETICION A LA API: {sw.ElapsedMilliseconds} ms |========|");
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult> GetProductsFromFilter([FromBody] FiltersRequestDto filters, [FromQuery] string term)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var products = await _scraperService.SearchProductsFilter(filters, term);

            sw.Stop();
            Console.WriteLine($"|========| DURACION DE PETICION A LA API APLICANDO FILTROS: {sw.ElapsedMilliseconds} ms |========|");
            return Ok(products);
        }
    }
}
