namespace NguyenThanhTin_2122110125.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
<<<<<<< HEAD
        public string Password { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;

=======

        public bool IsActive { get; set; } = true;

>>>>>>> d49e0555fd8c65a9b42f441054528926030c16bd
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public int? CreatedById { get; set; }

        public int? UpdatedById { get; set; }

        public int? DeletedById { get; set; }
    }
}
