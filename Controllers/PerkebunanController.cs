using Microsoft.AspNetCore.Mvc;
using Perkebunan.Data;
using Perkebunan.Models;
using Microsoft.EntityFrameworkCore;

namespace Perkebunan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerkebunanController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PerkebunanController(AppDbContext context)
        {
            _context = context;
        }   
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            return Ok(product);
        }
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromForm] ProductUploadDto dto)
        {
            if (dto.Image == null)
                return BadRequest("Image is required.");

            // Buat folder upload jika belum ada
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(),"uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            var product = new Product
            {
                NameProduct = dto.NameProduct,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = $"/uploads/{fileName}", // path yang akan diakses dari frontend
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("Product ID mismatch.");
            }
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            existingProduct.NameProduct = product.NameProduct;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.Stock = product.Stock;
            existingProduct.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("image/{filename}")]
        public IActionResult GetImage(string filename)
        {
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var filePath = Path.Combine(uploadsDir, filename);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var contentType = "image/" + Path.GetExtension(filename).TrimStart('.');

            return File(stream, contentType);
        }

    }
}
