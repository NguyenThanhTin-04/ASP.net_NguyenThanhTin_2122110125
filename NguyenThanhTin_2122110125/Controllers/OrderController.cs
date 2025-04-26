using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenThanhTin_2122110125.Model; 
using NguyenThanhTin_2122110125.Data;  

namespace NguyenThanhTin_2122110125.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext pro;

        public OrderController(AppDbContext context)
        {
            pro = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await pro.Orders
               
                .ToListAsync();
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await pro.Orders
      
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }


        [HttpPost]

        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
  
            if (string.IsNullOrEmpty(order.Name) || string.IsNullOrEmpty(order.Phone) ||
                string.IsNullOrEmpty(order.Email) || string.IsNullOrEmpty(order.Address))
            {
                return BadRequest("Các trường Name, Phone, Email và Address là bắt buộc.");
            }

            if (order.OrderDetails == null || !order.OrderDetails.Any())
            {
                return BadRequest("Đơn hàng phải có ít nhất một sản phẩm.");
            }


            foreach (var detail in order.OrderDetails)
            {
                var product = await pro.Products.FindAsync(detail.ProductId);
                if (product == null)
                {
                    return BadRequest($"Sản phẩm với ID {detail.ProductId} không tồn tại.");
                }

                if (product.Quantity < detail.Quantity)
                {
                    return BadRequest($"Sản phẩm '{product.Name}' không đủ hàng. Hiện có: {product.Quantity}, cần: {detail.Quantity}");
                }

      
                product.Quantity -= detail.Quantity;
                pro.Entry(product).State = EntityState.Modified;
            }

   
            pro.Orders.Add(order);
            await pro.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            pro.Entry(order).State = EntityState.Modified;

            try
            {
                await pro.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await pro.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            pro.Orders.Remove(order);
            await pro.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return pro.Orders.Any(e => e.Id == id);
        }

      
        [HttpGet("GetOrdersByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUserId(int userId)
        {
            var orders = await pro.Orders
                .Where(o => o.UserId == userId) 
                .Include(o => o.OrderDetails)   
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound("Không có đơn hàng nào của người dùng này.");
            }

            return orders;
        }

  
        [HttpGet("GetOrderDetail/{orderId}")]
        public async Task<ActionResult<Order>> GetOrderDetail(int orderId)
        {
            var order = await pro.Orders
                .Include(o => o.OrderDetails) 
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound($"Đơn hàng với ID {orderId} không tồn tại.");
            }

            return order; 
        }
    }

}
