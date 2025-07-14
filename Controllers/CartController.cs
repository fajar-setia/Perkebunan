using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Perkebunan.Data;
using Perkebunan.Models;
using System.Security.Claims;

namespace Perkebunan.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems()
        {
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Include(c => c.User)
                .ToListAsync();

            return Ok(cartItems);
        }

        // GET: api/Cart/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetCartItem(int id)
        {
            var cartItem = await _context.CartItems
                .Include(c => c.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cartItem == null)
            {
                return NotFound($"Cart item with ID {id} not found.");
            }

            return Ok(cartItem);
        }

        // POST: api/Cart
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CartItem>> AddToCart([FromBody] CartItemDTO dto)
        {
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                Console.WriteLine("❌ GAGAL: Claim 'sub' atau 'nameid' tidak ditemukan");
                return Unauthorized("User ID tidak ditemukan dari token.");
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var productExists = await _context.Products.AnyAsync(p => p.Id == dto.ProductId);
            if (!productExists)
                return BadRequest("Product tidak ditemukan");

            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return Ok(cartItem);
        }

        // PUT: api/Cart/5
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
            cartItem.CreatedAt = DateTime.UtcNow;

            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Cart/5
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

        // GET: api/Cart/user/123
        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItemsByUser(Guid userId)
        {
            Console.WriteLine($"✅ Menerima userId: {userId}");

            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Product)
                .Include(ci => ci.User)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                Console.WriteLine("⚠️ Tidak ada item keranjang ditemukan untuk user ini.");
                return NotFound($"Tidak ada item keranjang untuk user dengan ID {userId}.");
            }

            return Ok(cartItems);
        }

        // DELETE: api/Cart/user/123
        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> ClearCartByUser(Guid userId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                return NotFound($"Tidak ada item keranjang untuk user dengan ID {userId}.");
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Semua item keranjang untuk user {userId} telah dihapus." });
        }


    }
}
