using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductHub.Api.Contracts;
using ProductHub.Api.Models;

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
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsAsync()
    {
        var products = await _productsService.GetProductsAsync();
        return Ok(_mapper.Map<IEnumerable<ReadProductDto>>(products));
    }

    [HttpGet("{id}", Name="GetProduct")]
    public async Task<IActionResult> GetProductAsync(string id)
    {
        var product = await _productsService.GetProductAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<ReadProductDto>(product));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductDto createProductDto)
    {
        if (createProductDto == null)
        {
            return BadRequest();
        }

        var product = _mapper.Map<Product>(createProductDto);
        var response = await _productsService.AddProductAsync(product);
        var productReadDto = _mapper.Map<ReadProductDto>(response);

        return CreatedAtRoute("GetProduct", new { id = productReadDto.Id }, productReadDto);
    }

    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteProductAsync(string Id)
    {
        var product = _productsService.GetProductAsync(Id);

        if (product == null)
        {
            return NotFound();
        }

        if (Id == null)
        {
            throw new ArgumentNullException(nameof(Id));
        }

        await _productsService.DeleteProductAsync(Id);
        return NoContent();
    }
}
