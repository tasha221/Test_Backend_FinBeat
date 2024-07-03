using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using Test_Backend_FinBeat.Items;

namespace Test_Backend_FinBeat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValueController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ValueController> _logger;

        public ValueController(IConfiguration configuration, ILogger<ValueController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("SaveValues")]
        public async Task<IActionResult> SaveValues([FromBody] List<Dictionary<int, string>> data)
        {
            try
            {
                await using var connection = new SqlConnection(_configuration.GetConnectionString("MsDb"));
                await connection.ExecuteAsync("DELETE FROM dbo.fb_values", commandType: CommandType.Text);

                var valueItems = data.SelectMany(d => d.Select(kvp => new ValueItem
                {
                    Code = kvp.Key,
                    Value = kvp.Value
                }))
                .OrderBy(item => item.Code)
                .ToList();

                foreach (var item in valueItems)
                {
                    var sql = "INSERT INTO dbo.fb_values (Code, Value) VALUES (@Code, @Value)";
                    await connection.ExecuteAsync(sql, new { item.Code, item.Value });
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while saving values into dbo.fb_values");

                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetValues")]
        public async Task<IActionResult> GetData([FromQuery] int? code)
        {
            try
            {
                await using var connection = new SqlConnection(_configuration.GetConnectionString("MsDb"));

                string sql = "SELECT Id, Code, Value FROM dbo.fb_values";
                if (code.HasValue)
                {
                    sql += " WHERE Code = @Code";
                }

                var valueItems = await connection.QueryAsync<ValueItem>(sql, new { Code = code });

                return Ok(valueItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching values from dbo.fb_values");

                return StatusCode(500, "Internal server error");
            }
        }
    }
}