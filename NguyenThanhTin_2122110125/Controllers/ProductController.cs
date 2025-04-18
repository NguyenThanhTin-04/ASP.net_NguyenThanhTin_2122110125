using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using NguyenThanhTin_2122110125.Data;
using NguyenThanhTin_2122110125.Model;

namespace NguyenThanhTin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync(); //  Không Include Category
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id); //  Không Include

            if (product == null)
                return NotFound();

            return product;
        }

        // // GET: api/Product/ByCategory/3
        // [HttpGet("ByCategory/{categoryId}")]
        // public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
        // {
        //     return await _context.Products
        //                          .Where(p => p.CategoryId == categoryId)
        //                          .ToListAsync(); // ❌ Không Include
        // }

        // POST: api/Product
        // [HttpPost]
        // public async Task<ActionResult<Product>> PostProduct(Product product)
        // {
        //     _context.Products.Add(product);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        // }


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

            // Tạo thư mục images nếu chưa có
            var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }

            // Lưu ảnh vào thư mục wwwroot/images
            var fileName = Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(imageFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Tạo đường dẫn URL tương đối để lưu vào DB
            var imageUrl = fileName;

            // Tạo sản phẩm mới
            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                CategoryId = categoryId,
                ImageUrl = imageUrl,
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }


        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(p => p.Id == id))
                    return NotFound();
                else
                    throw;
            }

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
