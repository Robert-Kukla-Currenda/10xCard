namespace TenXCards.API.Controllers
{
    public class GetPagedListQuery
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public string? Sort { get; set; }
        public string? GeneratedBy { get; set; }
    }
}