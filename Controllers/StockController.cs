using AutoMapper;
using identity.DTO.Stock;
using identity.interfaces;
using Identity.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepo;
        private readonly IMapper _mapper;

        public StockController(IStockRepository stockRepo, IMapper mapper)
        {
            _stockRepo = stockRepo;
            _mapper = mapper;
        }

        [HttpPost("create")]
        [SwaggerOperation(
            Summary = "Create a new stock",
            Description = "Creates a new stock entry with the provided stock details."
        )]
        [SwaggerResponse(200, "Stock created successfully.")]
        [SwaggerResponse(400, "Bad request. Stock already exists or invalid data.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> CreateStock([FromBody] UpdateStockRequestDto stockdto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (await _stockRepo.GetBySymbolAsync(stockdto.Symbol) != null)
                    return BadRequest("Stock already exists");

                var stock = _mapper.Map<Stock>(stockdto);
                await _stockRepo.CreateAsync(stock);

                return Ok("Stock created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Update an existing stock",
            Description = "Updates the details of an existing stock based on the provided stock ID."
        )]
        [SwaggerResponse(200, "Stock updated successfully.")]
        [SwaggerResponse(400, "Bad request. Invalid data or stock not found.")]
        [SwaggerResponse(500, "Internal server error.")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockRequestDto stockdto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var stock = await _stockRepo.GetByIdAsync(id);
                if (stock == null)
                    return NotFound("Stock not found");

                _mapper.Map(stockdto, stock);
                await _stockRepo.UpdateAsync(stock);

                return Ok("Stock updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get all stocks",
            Description = "Retrieves a list of all available stocks."
        )]
        [SwaggerResponse(200, "List of stocks retrieved successfully.", typeof(List<Stock>))] // Replace Stock with your actual model
        [SwaggerResponse(204, "No stocks found.")]
        public async Task<IActionResult> GetAllStocks()
        {
            var stocks = await _stockRepo.GetAllAsync();
            if (stocks.Count < 1)
                return StatusCode(StatusCodes.Status204NoContent, new { message = "no stock(s) found" });
            return Ok(stocks);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Get stock by ID",
            Description = "Retrieves stock details based on the provided stock ID."
        )]
        [SwaggerResponse(200, "Stock retrieved successfully.", typeof(Stock))] // Replace Stock with your actual model
        [SwaggerResponse(404, "Stock not found.")]
        public async Task<IActionResult> GetStockById(int id)
        {
            var stock = await _stockRepo.GetByIdAsync(id);
            if (stock == null)
                return NotFound("Stock not found");
            return Ok(stock);
        }

        [HttpGet("symbol/{symbol}")]
        
        [SwaggerOperation(
            Summary = "Gets a stock by symbol",
            Description = "Retrieves stock details based on the provided stock symbol."
        )]
        [SwaggerResponse(200, "Stock retrieved successfully.", typeof(Stock))] // Replace Stock with your actual model
        [SwaggerResponse(404, "Stock not found.")]
        public async Task<IActionResult> GetStockBySymbol(string symbol)
        {
            var stock = await _stockRepo.GetBySymbolAsync(symbol);
            if (stock == null)
                return NotFound("Stock not found");
            return Ok(stock);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Deletes a stock",
            Description = "Removes a stock from the database based on the provided stock ID."
        )]
        [SwaggerResponse(200, "Stock deleted successfully.")]
        [SwaggerResponse(404, "Stock not found.")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            if (!await _stockRepo.StockExists(id))
                return NotFound("Stock not found");
            await _stockRepo.DeleteAsync(id);
            return Ok("Stock deleted successfully");
        }
    }
}
