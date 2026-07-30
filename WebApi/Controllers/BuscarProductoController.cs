using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scrapers.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/productos")]
    public class BuscarProductoController : ControllerBase
    {
        //private readonly ISearchService _scraperService;
        private readonly ScraperService _scraperServicio;
        private readonly ValidacionesRequests _validacionesRequests;

        public BuscarProductoController(ScraperService scraperServicio, ValidacionesRequests validacionesRequests)
        {
            //_scraperService = scraperService;
            _scraperServicio = scraperServicio;
            _validacionesRequests = validacionesRequests;
        }

        [HttpGet]
        public async Task<ActionResult> ObtenerProducto([FromQuery][Required(ErrorMessage = "El producto a buscar es obligatorio.")] string productoBuscar)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var products = await _scraperServicio.ObtenerProductosAsync(productoBuscar);
            
            sw.Stop();
            Console.WriteLine($"|========| DURACION DE PETICION A LA API: {sw.ElapsedMilliseconds} ms |========|");
            return Ok(products);
        }

        //[HttpPost]
        //public async Task<ActionResult> GetProductsFromFilter([FromBody] FiltersRequestDto filters, [FromQuery] string term)
        //{
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();
        //    var products = await _scraperService.SearchProductsFilter(filters, term);

        //    sw.Stop();
        //    Console.WriteLine($"|========| DURACION DE PETICION A LA API APLICANDO FILTROS: {sw.ElapsedMilliseconds} ms |========|");
        //    return Ok(products);
        //}
    }
}
