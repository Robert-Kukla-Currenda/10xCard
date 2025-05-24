namespace TenXCards.Frontend.Models
{
    public class PaginationInfo
    {
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int TotalItems { get; set; } = 0;
        public int ItemsPerPage { get; set; } = 10;
    }
}