namespace NguyenThanhTin_2122110125.Model
{
    public class RemoveCartItemsRequest
    {
        public int UserId { get; set; }
        public List<int> ProductIds { get; set; }
    }
}
