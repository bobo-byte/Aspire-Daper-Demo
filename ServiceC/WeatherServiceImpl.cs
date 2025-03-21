using System.Text.Json;
using Dapr.Client;
using Grpc.Core;
using Shared.Protos.v1;

namespace ServiceC;

public class WeatherServiceImpl(
    DaprClient daprClient) : WeatherService.WeatherServiceBase
{
    public override async Task<WeatherResponse> GetWeather(
        WeatherRequest request,
        ServerCallContext context)
    {
        // 1. Call ServiceB (HTTP) via Dapr
        var serviceBResponse = await daprClient.InvokeMethodAsync<object>(
            HttpMethod.Get,
            "service-b",
            $"api/weather/{request.City}"
        );
        var serviceBData =
            JsonSerializer.Deserialize<Dictionary<string, object>>(
                JsonSerializer.Serialize(serviceBResponse));
        var serviceBTemp = Convert.ToDouble(serviceBData?["Temperature"]);

        // 2. Call ServiceA (gRPC) via Dapr
        var serviceARequest = new WeatherRequest { City = request.City };
        var serviceAResponse = await daprClient
            .InvokeMethodGrpcAsync<WeatherRequest, WeatherResponse>(
                "service-a",
                "weather.v1.WeatherService/GetWeather",
                serviceARequest
            );
        var serviceATemp = serviceAResponse.Temperature;

        // 3. Aggregate the results (e.g., average the temperatures)
        var averageTemp = (serviceATemp + serviceBTemp) / 2;

        return new WeatherResponse
        {
            City = request.City,
            Temperature = averageTemp
        };
    }
}