using Grpc.Core;
using Shared.Protos.v1;

namespace ServiceA;

public class WeatherServiceImpl : WeatherService.WeatherServiceBase
{
    public override Task<WeatherResponse> GetWeather(WeatherRequest request,
        ServerCallContext context)
    {
        return Task.FromResult(new WeatherResponse
        {
            City = request.City,
            Temperature = 20
        });
    }
}