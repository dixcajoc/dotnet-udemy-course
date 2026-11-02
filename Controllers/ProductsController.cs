using ApiEcommerce.Models;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public ProductsController(
            IProductRepository productRepository, 
            ICategoryRepository categoryRepository, 
            IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpGet("{productId:int}", Name = "GetProductById")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductById(int productId)
        {   
            var product = _productRepository.GetProductById(productId);
            if(product == null)
            {
                return NotFound($"El producto con el id {productId} no existe");
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (createProductDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_productRepository.ProductExists(createProductDto.Name))
            {
                ModelState.AddModelError("CustomError", "El producto ya existe");
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"La categoria con el {createProductDto.CategoryId} no existe");
                return BadRequest(ModelState);
            }


            var product = _mapper.Map<Product>(createProductDto);

            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Ocurrió un error al crear el producto {product.Name}");
                return StatusCode(500,ModelState);
            }

            var createdProduct = _productRepository.GetProductById(product.ProductId);
            var productDto = _mapper.Map<ProductDto>(createdProduct);
            return CreatedAtRoute("GetProductById", new { productId = product.ProductId }, productDto);
        }

        [HttpGet("searchProductsByCategory{categoryId:int}", Name = "GetProductsForCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProductForCategory(int categoryId)
        {   
            var products = _productRepository.GetProductsForCategory(categoryId);
            if(products.Count == 0)
            {
                return NotFound($"Los productos con la categoría {categoryId} no existen");
            }

            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        // Para los parametros de ruta de tipo string no se especifica el tipo de dato
        [HttpGet("searchProductsByNameDescripttion{searchTerm}", Name = "SearchProducts")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult SearchProducts(string searchTerm)
        {   
            var products = _productRepository.SearchProducts(searchTerm);
            if(products.Count == 0)
            {
                return NotFound($"Los productos con el nombre {searchTerm} no existen");
            }

            var productsDto = _mapper.Map<List<ProductDto>>(products);
            return Ok(productsDto);
        }

        [HttpPatch("buyProduct/{productName}/{quantity}", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult BuyProduct(string productName, int quantity)
        {   
            if (string.IsNullOrWhiteSpace(productName) || quantity <= 0)
            {
                return BadRequest("El nombre del producto no puede estar vacío y la cantidad debe ser mayor a cero");
            }
            var foundProduct = _productRepository.ProductExists(productName);
            if (!foundProduct)
            {
                return NotFound($"El producto con el nombre {productName} no existe");
            }

            if (!_productRepository.BuyProduct(productName, quantity))
            {
                ModelState.AddModelError("CustomError", $"No se pudo comprar el producto {productName} o la cantidad solicitada es mayor a la disponible");

                return BadRequest(ModelState);
            }

            var units = quantity == 1 ? "unidad" : "unidades";

            return Ok($"Se compró {quantity} {units} del producto {productName} con éxito");
        }

        [HttpPut("{productId:int}", Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProduct(int productId, [FromBody] UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null)
            {
                return BadRequest(ModelState);
            }
            var product = _productRepository.GetProductById(productId);
            if (product == null)
            {
                ModelState.AddModelError("CustomError", $"El producto con  id {productId} no existe");
                return BadRequest(ModelState);
            }

            if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
            {
                ModelState.AddModelError("CustomError", $"La categoria con el {updateProductDto.CategoryId} no existe");
                return BadRequest(ModelState);
            }


            _mapper.Map(updateProductDto, product);

            if (!_productRepository.UpdateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Ocurrió un error al actualizar el producto {product.Name}");
                return StatusCode(500,ModelState);
            }

            return NoContent();
        }
        
        
        [HttpDelete("{productId:int}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteProduct(int productId)
        {  
            var product = _productRepository.GetProductById(productId);
            if(product == null)
            {
                return NotFound($"El producto con el id {productId} no existe");
            }

            if (!_productRepository.DeleteProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Ocurrió un error al eliminar el producto {product.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

    }

}
