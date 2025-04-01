using Identity.DTO.Stock;
using Identity.Models;

namespace identity.interfaces
{
    public interface IPortfolioRepository
    {
        Task<List<ListStockDTO>> GetUserPorfolio(AppUser user);
        Task<bool> CreatePortfolio(AppUser user, Stock stock);


        Task<bool> IsStockInPortfolio(AppUser user, Stock stock);

        Task<bool> DeletePortfolio(AppUser appUser,Stock stock);
        Task<bool> SaveAsync();
    }
}