using TenXCards.API.Controllers;
using TenXCards.API.Models;

namespace TenXCards.API.Services
{
    public interface IOriginalContentService
    {
        Task<OriginalContentDto> CreateAsync(CreateOriginalContentCommand command, int userId);
        Task<OriginalContentDto> GetContentByIdAsync(int id, int userId);
        Task<(List<OriginalContentDto> Items, int Total)> GetUserContentsAsync(GetPagedListQuery getListQuery, int userId);
        Task DeleteContentAsync(int id, int userId);
    }
}