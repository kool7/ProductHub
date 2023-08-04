using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductHub.Application.Contracts.Products;
using ProductHub.Domain.Products;

namespace ProductHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductsService _productsService;
    private readonly IMapper _mapper;

    public ProductsController(
        ILogger<ProductsController> logger,
        IProductsService productsService,
        IMapper mapper)
    {
        _logger = logger;
        _productsService = productsService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsAsync([FromQuery] int pageNumber = 1,
                                                                           [FromQuery] int pageSize = 10,
                                                                           [FromQuery] string searchTerm = "",
                                                                           [FromQuery] string sort = "asc")
    {
        var products = await _productsService.GetProductsAsync(pageNumber, pageSize, searchTerm, sort);
        return Ok(products);
    }

    [HttpGet("{Id}", Name = "GetProduct")]
    public async Task<IActionResult> GetProductAsync(string Id)
    {
        var product = await _productsService.GetProductAsync(Id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductDto createProductDto)
    {
        var product = _mapper.Map<Product>(createProductDto);
        var createdProduct = await _productsService.AddProductAsync(product);
        return CreatedAtRoute("GetProduct", new { id = createdProduct.Id }, createdProduct);
    }

    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateProductAsync(string Id, [FromBody] UpdateProductDto updateProductDto)
    {
        var product = _mapper.Map<Product>(updateProductDto, opts => opts.Items["Id"] = Id);
        var updatedProduct = await _productsService.UpdateProductAsync(Id, product);
        return Ok(updatedProduct);
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteProductAsync(string Id)
    {
        await _productsService.DeleteProductAsync(Id);
        return NoContent();
    }
}
