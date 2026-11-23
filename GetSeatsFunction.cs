using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HotelingFunctionApp;

public class GetSeatsFunction
{
    private readonly ILogger<GetSeatsFunction> _logger;
    private readonly IBusinessLogic _businessLogic;

    public GetSeatsFunction(ILogger<GetSeatsFunction> logger, IBusinessLogic businessLogic)
    {
        _logger = logger;
        _businessLogic = businessLogic;
    }

    [Function("GetSeatsFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        // Use business logic to obtain seats and always return JSON
        try
        {
            var seats = await _businessLogic.GetSeatsBusinessLogicAsync();
            return new OkObjectResult(seats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting seats");
            var error = new { message = "An error occurred while retrieving seats." };
            return new ObjectResult(error) { StatusCode = 500 };
        }
    }
}