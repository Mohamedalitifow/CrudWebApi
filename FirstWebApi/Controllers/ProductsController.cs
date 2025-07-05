using FirstWebApi.Data;
using FirstWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FirstWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // Get : api/products
        [HttpGet]

        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _context.Products.ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving products",
                    error = ex.Message
                });
            }
        }

        // Get ById : api/products/id
        [HttpGet("{id}")]
       public async Task<IActionResult> GetProduct(int id)
        {

            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null )
                {
                    return NotFound(new
                    {
                        message = $"Product with ID {id} not found"
                    });
                }
                return Ok(product);

            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error retrieving product!",
                    error = ex.Message
                });
            }
        }


        // Post : api/products
        [HttpPost]

        public async Task<IActionResult> PostProduct([FromForm] Product product)
        {
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error creating product",
                    error = ex.Message
                });
            }
        }

        // Update : api/product/id
        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateProduct(int id , Product product )
        {
           if (id != product.Id)
            {

                return BadRequest(new
                {
                    message = "Product ID mismatch"
                });
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    message = "Product updated successfully",
                    product
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.Id == id))
                {
                    return NotFound(new
                    {
                        message = $"Product with ID {id} not found for update"
                    });
                }

                return StatusCode(500, new
                {
                    message = "Concurrency error occurred while updating"
                });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error updating product",
                    error = ex.Message
                });
            }


        }


        // Delete : api/products/id
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteProducts(int id )
        {
             try
            {
                var product = await _context.Products.FindAsync(id);
                if ( product == null)
                {
                    return NotFound(new
                    {
                        message = $"Product with ID {id} not found",
                    });
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Product deleted successfully",
                    deletedProduct = product
                });
            }
            catch (Exception ex )
            {

                return StatusCode(500, new
                {
                    message = "Error deleting product",
                    error = ex.Message
                });
            }
        }


    }
}
