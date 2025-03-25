using Microsoft.AspNetCore.Mvc;
using Shared.Protos.v1;

namespace ServiceA;

[Route("api/[controller]")]
[ApiController]
public class WeatherController : ControllerBase
{
    [HttpGet("{city}")]
    public ActionResult<WeatherResponse> Get(string city)
    {
        return Ok(new WeatherResponse
        {
            City = city,
            Temperature = 20
        });
    }
}