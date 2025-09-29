using Microsoft.AspNetCore.Mvc;
using SearchService.Application.Interfaces;

namespace SearchService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] object product)
        {
            await _searchService.IndexDocumentAsync(product, Guid.NewGuid().ToString());
            return Ok();
        }

        [HttpGet("{query}")]
        public async Task<IActionResult> Search(string query)
        {
            var reuslt = await _searchService.SearchAsync<object>(query);
            return Ok(reuslt);
        }
    }
}
