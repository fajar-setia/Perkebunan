using Microsoft.AspNetCore.Mvc;

namespace Perkebunan.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    [HttpGet]
    [Route("{*filename}")]
    public async Task<IActionResult> GetFile(string filename)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", filename);
        
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound($"File '{filename}' not found.");
        }
        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/octet-stream", filename);
    }
}
