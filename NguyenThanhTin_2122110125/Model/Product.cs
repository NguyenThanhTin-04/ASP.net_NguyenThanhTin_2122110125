namespace NguyenThanhTin_2122110125.Model
{
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }

        public DateTime CreatedAt { get; set; }     
        public string CreatedBy { get; set; }       
        public DateTime? UpdatedAt { get; set; }   
        public string UpdatedBy { get; set; }       
        public bool IsDeleted { get; set; }         
        public DateTime? DeletedAt { get; set; }    
        public string DeletedBy { get; set; }      
    }
}
