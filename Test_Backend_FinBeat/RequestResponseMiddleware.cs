using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Dapper;
using System.IO;
using System.Threading.Tasks;
using Test_Backend_FinBeat.Items;
using Test_Backend_FinBeat.Controllers;

namespace Test_Backend_FinBeat
{
    public class RequestResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RequestResponseMiddleware> _logger;

        public RequestResponseMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<RequestResponseMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                var request = await FormatRequest(context.Request);

                var originalBodyStream = context.Response.Body;
                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

                    await _next(context);

                    var response = await FormatResponse(context.Response);
                    await LogRequestAndResponse(context, request, response);

                    await responseBody.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in RequestResponseLoggingMiddleware");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal server error");
            }
        }

        private async Task LogRequestAndResponse(HttpContext context, string request, string response)
        {
            var connectionString = _configuration.GetConnectionString("MsDb");

            using (var connection = new SqlConnection(connectionString))
            {
                var sql = @"
                INSERT INTO dbo.fb_requestLogs (Method, Path, RequestBody, ResponseBody, StatusCode, Timestamp)
                VALUES (@Method, @Path, @RequestBody, @ResponseBody, @StatusCode, @Timestamp)";
                var log = new RequestLogItem
                {
                    Method = context.Request.Method,
                    Path = context.Request.Path,
                    RequestBody = request,
                    ResponseBody = response,
                    StatusCode = context.Response.StatusCode,
                    Timestamp = DateTime.Now
                };

                await connection.ExecuteAsync(sql, log);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.Body.Position = 0;
            var body = await new StreamReader(request.Body, leaveOpen: true).ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return text;
        }
    }
}
