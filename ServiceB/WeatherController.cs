using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Shared.Protos.v1;

namespace ServiceB;

[Route("api/[controller]")]
[ApiController]
public class WeatherController(
    DaprClient daprClient) : ControllerBase
{
    [HttpGet("{city}")]
    public async Task<IActionResult> Get(string city)
    {
        var response = await daprClient.InvokeMethodAsync<WeatherResponse>(
            HttpMethod.Get, "service-a", $"weather/{city}");
        return Ok(response);
    }
}