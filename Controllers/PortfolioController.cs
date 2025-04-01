using AutoMapper;
using identity.DTO.Stock;
using identity.interfaces;
using Identity.DTO.Stock;
using Identity.Extensions;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IStockRepository _stockRepo;
        private readonly IPortfolioRepository _portfolioRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public PortfolioController(UserManager<AppUser> userManager, IPortfolioRepository portfolioRepo, IMapper mapper, IStockRepository stockRepo)
        {
            _portfolioRepo = portfolioRepo;
            _mapper = mapper;
            _stockRepo = stockRepo;
            _userManager = userManager;
        }
        [HttpGet("portfolios")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Retrieves the authenticated user's portfolio",
            Description = "Fetches all stocks in the authenticated user's portfolio."
        )]
        [SwaggerResponse(200, "Returns the user's portfolio.", typeof(List<StockDTO>))]
        [SwaggerResponse(400, "User not found.")]
        [SwaggerResponse(401, "Unauthorized user.")]
        [SwaggerResponse(404, "User portfolio is empty.")]
        public async Task<IActionResult> GetUserPorfolio()
        {
            // Get the username of the authenticated user
            var username = User.GetUsername();
            // Check if the user is authenticated
            if (username == null)
                return StatusCode(StatusCodes.Status401Unauthorized, "User Not Authenticated");
            // Get the user from the database
            var appUser = await _userManager.FindByNameAsync(username);
            // Check if the user exists
            if (appUser == null)
                return StatusCode(StatusCodes.Status400BadRequest, "User Not Found");
            // Get the user's portfolio
            var userPortfolio = await _portfolioRepo.GetUserPorfolio(appUser);
            // Check if the portfolio is empty
            if (userPortfolio == null || !userPortfolio.Any())
                return NotFound("User portfolio is empty.");

            return Ok(userPortfolio);
        }



        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Adds a stock to the user's portfolio",
                  Description = "Allows an authenticated user to add a stock to their portfolio.")]
        [SwaggerResponse(201, "Stock successfully added.")]
        [SwaggerResponse(400, "Invalid request.")]
        [SwaggerResponse(401, "Unauthorized user.")]
        [SwaggerResponse(404, "Stock not found.")]
        [SwaggerResponse(500, "Server error.")]
        public async Task<IActionResult> CreatePortfolio([FromQuery] string stockSymbol)
        {
            // Get the username of the authenticated user
            var username = User.GetUsername();
            if (username == null)
                return Unauthorized(new { message = "User Not Authenticated" });

            // Get the user from the database
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
                return BadRequest(new { message = "User Not Found" });

            // Validate stock symbol
            if (string.IsNullOrEmpty(stockSymbol))
                return BadRequest(new { message = "Stock Symbol is required" });

            // Get the stock from the database
            var stock = await _stockRepo.GetBySymbolAsync(stockSymbol);
            if (stock == null)
                return NotFound(new { message = "Stock not found" });

            // Check if the stock already exists in the user's portfolio
            if (await _portfolioRepo.IsStockInPortfolio(appUser, stock))
                return BadRequest(new { message = "Stock already exists in user's portfolio" });

            // Add the stock to the user's portfolio
            var result = await _portfolioRepo.CreatePortfolio(appUser, stock);
            if (result)
                return CreatedAtAction(nameof(GetUserPorfolio), new { username = appUser.UserName }, new { message = $"{stock.CompanyName} added to user's portfolio" });

            // Handle unexpected errors

            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while adding stock to user's portfolio" });
        }
        
        [HttpDelete("delete/{stockSymbol}")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Removes a stock from the user's portfolio",
            Description = "Deletes the specified stock from the authenticated user's portfolio."
        )]
        [SwaggerResponse(200, "Stock successfully deleted.")]
        [SwaggerResponse(400, "Invalid request (e.g., stock symbol missing or stock not in portfolio).")]
        [SwaggerResponse(401, "Unauthorized user.")]
        [SwaggerResponse(404, "Stock not found.")]
        [SwaggerResponse(500, "Server error.")]
        public async Task<IActionResult> DeletePorfolio([FromRoute] string stockSymbol)
        {
            // Get the username of the authenticated user
            var username = User.GetUsername();
            if (username == null)
                return Unauthorized(new { message = "User Not Authenticated" });

            // Get the user from the database
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
                return BadRequest(new { message = "User Not Found" });

            // Validate stock symbol
            if (string.IsNullOrEmpty(stockSymbol))
                return BadRequest(new { message = "Stock Symbol is required" });

            // Get the stock from the database
            var stock = await _stockRepo.GetBySymbolAsync(stockSymbol);
            if (stock == null)
                return NotFound(new { message = "Stock not found" });

            // Check if the stock exists in the user's portfolio
            if (!await _portfolioRepo.IsStockInPortfolio(appUser, stock))
                return BadRequest(new { message = "Stock does not exist in user's portfolio" });

            // Delete the stock from the user's portfolio
            var result = await _portfolioRepo.DeletePortfolio(appUser, stock);
            if (result)
                return CreatedAtAction(nameof(GetUserPorfolio), new { username = appUser.UserName }, new { message = "Stock deleted from user's portfolio" });

            // Handle unexpected errors

            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting stock from user's portfolio" });
        }
    }
}