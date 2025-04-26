using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenThanhTin_2122110125.Data;
using NguyenThanhTin_2122110125.Model;

namespace NguyenThanhTin_2122110125.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _uploadFolder;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _uploadFolder = Path.Combine(env.WebRootPath, "images");

            // Nếu thư mục images chưa tồn tại thì tạo
            if (!Directory.Exists(_uploadFolder))
            {
                Directory.CreateDirectory(_uploadFolder);
            }
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound();

            return product;
        }

        // POST: api/Product
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(
            [FromForm] string name,
            [FromForm] string description,
            [FromForm] double price,
            [FromForm] int categoryId,
            [FromForm] IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("Ảnh không hợp lệ.");
            }

            var fileName = Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(_uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                CategoryId = categoryId,
                ImageUrl = fileName
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id,
            [FromForm] string name,
            [FromForm] string description,
            [FromForm] double price,
            [FromForm] int categoryId,
            [FromForm] int quantity,
            [FromForm] IFormFile? imageFile)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            product.Name = name;
            product.Description = description;
            product.Price = price;
            product.Quantity = quantity;
            product.CategoryId = categoryId;

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(_uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                product.ImageUrl = fileName;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
