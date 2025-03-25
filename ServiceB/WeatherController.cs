using Dapr.Client;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Shared.Protos.v1;

namespace ServiceB;

[Route("api/[controller]")]
[ApiController]
public class WeatherController(
    DaprClient daprClient) : ControllerBase
{
    [HttpGet("{city}")]
    public async Task<ActionResult<WeatherResponse>>
        Get(string city, CancellationToken cancellationToken)
    {
        var serviceAClient =
            new DaprClientBuilder()
                .UseGrpcChannelOptions(new GrpcChannelOptions
                {
                    // Has to be explicit if using Http1AndHttp2 Kestrel Protocol
                    HttpVersion = new Version(2, 0)
                })
                .Build();

        var serviceAInvokeRequest =
            serviceAClient.CreateInvokeMethodRequest(
                HttpMethod.Post,
                "service-a",
                "weather.v1.WeatherService/GetWeather",
                new List<KeyValuePair<string, string>>(),
                new WeatherRequest
                {
                    City = city
                });

        using var serviceAHttpClient =
            serviceAClient.CreateInvokableHttpClient();
        var response =
            await (await serviceAHttpClient.SendAsync(serviceAInvokeRequest,
                    cancellationToken))
                .Content
                .ReadFromJsonAsync<WeatherResponse>(
                    cancellationToken: cancellationToken);

        if (response is null)
        {
            return NotFound();
        }

        return response;
    }
}