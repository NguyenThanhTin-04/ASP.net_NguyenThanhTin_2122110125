namespace NguyenThanhTin_2122110125.Model
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }

        public int? Quantity { get; set; }
    }
}
