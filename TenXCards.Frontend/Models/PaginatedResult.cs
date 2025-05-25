namespace TenXCards.Frontend.Models;

public class PaginationResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();    
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < Total;
}
