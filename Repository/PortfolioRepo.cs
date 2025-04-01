using identity.interfaces;
using identity.Models.JoinedTable;
using Identity.Data;
using Identity.DTO.Stock;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace identity.Repository
{
    public class PortfolioRepo : IPortfolioRepository
    {
        private readonly ApplicationDBContext _context;

        public PortfolioRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<bool> CreatePortfolio(AppUser user, Stock stock)
        {
            var portfolio = new Portfolio
            {
                AppUserId = user.Id,
                StockId = stock.Id,

            };
            _context.Portfolios.Add(portfolio);
            await SaveAsync();

            return true; // Portfolio successfully created

        }

        public async Task<bool> DeletePortfolio(AppUser appuser, Stock stock)
        {
            var portfolio = await _context.Portfolios
                .Where(p => p.StockId == stock.Id && p.AppUserId == appuser.Id)
                .FirstOrDefaultAsync();

            if (portfolio == null)
            {
                return false; // Portfolio does not exist
            }

            _context.Portfolios.Remove(portfolio);
            await SaveAsync();

            return true; // Portfolio successfully deleted
        }


        public async Task<List<ListStockDTO>> GetUserPorfolio(AppUser user)
        {
            var stocks = await _context.Portfolios.Where(u => u.AppUserId == user.Id).Select(stock => new ListStockDTO
            {
                Id = stock.Stock.Id,
                Symbol = stock.Stock.Symbol,
                CompanyName = stock.Stock.CompanyName,
                Purchase = stock.Stock.Purchase,
                LastDiv = stock.Stock.LastDiv,
                Industry = stock.Stock.Industry,
                MarketCap = stock.Stock.MarketCap,
            }).ToListAsync();

            return stocks;
        }

        public Task<bool> IsStockInPortfolio(AppUser user, Stock stock)
        {
            return Task.FromResult(_context.Portfolios.Any(p => p.AppUserId == user.Id && p.StockId == stock.Id));
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}