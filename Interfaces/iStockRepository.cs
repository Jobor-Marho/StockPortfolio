using identity.DTO.Stock;
using identity.Models.JoinedTable;
using Identity.Models;

namespace identity.interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllAsync();
        Task<Stock?> GetByIdAsync(int id);
        Task<Stock?> GetBySymbolAsync(string symbol);
        Task<bool> CreateAsync(Stock stockModel);
        Task<bool> UpdateAsync(Stock updatedStock);
        Task<bool> DeleteAsync(int id);
        Task<bool> StockExists(int id);
        Task<bool> SaveAsync();

    }
}