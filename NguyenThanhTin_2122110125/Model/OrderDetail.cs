namespace NguyenThanhTin_2122110125.Model
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public int? Quantity { get; set; }


    }
}
