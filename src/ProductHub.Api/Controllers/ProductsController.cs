using Microsoft.AspNetCore.Mvc;
using ProductHub.Domain.Product;

namespace ProductHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        ILogger<ProductsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsAsync()
    {
        return Ok();
    }

    [HttpGet("{Id}", Name = "GetProduct")]
    public async Task<IActionResult> GetProductAsync(string Id)
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync([FromBody] Product product)
    {
        if (product == null)
        {
            return BadRequest();
        }

        return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
    }

    [HttpPut("{Id}")]
    public async Task<ActionResult> UpdateProductAsync(Guid Id, [FromBody] Product product)
    {
        return NoContent();
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteProductAsync(string Id)
    {
        return NoContent();
    }
}
