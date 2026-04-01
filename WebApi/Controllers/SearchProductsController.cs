using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            var products = await _scraperService.SearchAll(term);
            return Ok(products);
        }
    }
}
