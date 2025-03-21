using Microsoft.AspNetCore.Mvc;

namespace ServiceA;

[Route("api/[controller]")]
[ApiController]
public class WeatherController : ControllerBase
{
    [HttpGet("{city}")]
    public IResult Get(string city)
    {
        return Results.Ok(new
            { City = city, Temperature = 25.5 });
    }
}