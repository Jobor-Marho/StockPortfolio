using AutoMapper;
using identity.DTO.Stock;
using identity.interfaces;
using Identity.Data;
using Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace identity.Repository
{
    public class StockRepo : IStockRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        public StockRepo(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<bool> CreateAsync(Stock stockModel)
        {
            var stock = new Stock
            {
                Symbol = stockModel.Symbol,
                CompanyName = stockModel.CompanyName,
                Purchase = stockModel.Purchase,
                LastDiv = stockModel.LastDiv,
                Industry = stockModel.Industry,
                MarketCap = stockModel.MarketCap
            };
            _context.Stocks.Add(stock);
            await SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var stock = _context.Stocks.FirstOrDefault(s => s.Id == id);
            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Stock>> GetAllAsync()
        {
            var stocks = await _context.Stocks.ToListAsync();
            return stocks;
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.Symbol == symbol);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;

        }

        public async Task<bool> StockExists(int id)
        {
            return await _context.Stocks.AnyAsync(s => s.Id == id);
        }



        public async Task<bool> UpdateAsync(Stock updatedStock)
        {
            var stock = _context.Stocks.FirstOrDefaultAsync(s => s.Id == updatedStock.Id);
            if (stock != null)
            {
                stock.Result.Symbol = updatedStock.Symbol;
                stock.Result.CompanyName = updatedStock.CompanyName;
                stock.Result.Purchase = updatedStock.Purchase;
                stock.Result.LastDiv = updatedStock.LastDiv;
                stock.Result.Industry = updatedStock.Industry;
                stock.Result.MarketCap = updatedStock.MarketCap;
                await SaveAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}