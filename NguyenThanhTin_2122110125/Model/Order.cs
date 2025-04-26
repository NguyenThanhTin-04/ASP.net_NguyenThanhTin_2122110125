namespace NguyenThanhTin_2122110125.Model
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }

    }
}
