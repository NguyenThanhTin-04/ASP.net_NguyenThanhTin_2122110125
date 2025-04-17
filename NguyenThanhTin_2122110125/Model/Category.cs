namespace NguyenThanhTin_2122110125.Model
{
    public class Category
    {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }
            public DateTime? DeletedAt { get; set; }

            //public ICollection<Product> Products { get; set; }
    }
}
