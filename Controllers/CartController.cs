using Microsoft.AspNetCore.Mvc;
using Perkebunan.Data;
using Perkebunan.Models;
using Microsoft.EntityFrameworkCore;


namespace Perkebunan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CartController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems()
        {
            var cartItems = await _context.CartItems.Include(c => c.Product).Include(c => c.User).ToListAsync();
            return Ok(cartItems);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetCartItem(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound($"Cart item with ID {id} not found.");
            }
            return Ok(cartItem);
        }
        [HttpPost]
        public async Task<ActionResult<CartItem>> AddToCart([FromBody] CartItem cartItem)
        {
            cartItem.CreatedAt = DateTime.UtcNow;
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            // Ambil ulang data lengkap termasuk relasi
            var savedItem = await _context.CartItems
                .Include(c => c.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == cartItem.Id);

            return CreatedAtAction(nameof(GetCartItemsByUser), new { userId = savedItem.UserId }, savedItem);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, [FromBody] CartItem updatedCartItem)
        {
            if (id != updatedCartItem.Id)
            {
                return BadRequest("Cart item ID mismatch.");
            }
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound($"Cart item with ID {id} not found.");
            }
            cartItem.Quantity = updatedCartItem.Quantity;
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound($"Cart item with ID {id} not found.");
            }
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItemsByUser(Guid userId)
        {
            var cartItems = await _context.CartItems.Where(ci => ci.UserId == userId).Include(ci => ci.Product).Include(ci => ci.User).ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                return NotFound($"No cart items found for user with ID {userId}.");
            }

            return Ok(cartItems);
        }
        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> ClearCartByUser(Guid userId)
        {
            var cartItems = await _context.CartItems.Where(ci => ci.UserId == userId).Include(ci => ci.Product).Include(ci => ci.User).ToListAsync();
            if (cartItems == null || !cartItems.Any())
            {
                return NotFound($"No cart items found for user with ID {userId}.");
            }
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return Ok(cartItems);
        }
    }
}
