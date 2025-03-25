using Dapr.Client;
using Grpc.Core;
using Grpc.Net.Client;
using Shared.Protos.v1;

namespace ServiceC;

public class WeatherServiceImpl(
    DaprClient daprClient,
    IConfiguration configuration) : WeatherService.WeatherServiceBase
{
    public override async Task<WeatherResponse> GetWeather(
        WeatherRequest request,
        ServerCallContext context)
    {
        // 1. Call ServiceB (HTTP) via Dapr
        var serviceBResponse =
            await daprClient.InvokeMethodAsync<WeatherResponse>(
                HttpMethod.Get,
                "service-b",
                $"api/weather/{request.City}"
            );

        var serviceBData = serviceBResponse;

        // 2. Call ServiceA (gRPC) via Dapr
        var serviceAClient =
            new DaprClientBuilder()
                .UseGrpcChannelOptions(new GrpcChannelOptions
                {
                    // Has to be explicit if using Http1AndHttp2 Kestrel Protocol
                    HttpVersion = new Version(2, 0)
                })
                .Build();

        var serviceAInvokeRequest =
            serviceAClient.CreateInvokeMethodRequest("service-a",
                "weather.v1.WeatherService/GetWeather", request);
        using var serviceAHttpClient =
            serviceAClient.CreateInvokableHttpClient();
        var serviceAResponse =
            await (await serviceAHttpClient.SendAsync(serviceAInvokeRequest))
                .Content
                .ReadFromJsonAsync<WeatherResponse>();

        // Note:
        // if service is not found then try enabling proxy feature in config.yml

        // Alternative 1:
        // Make sure service implementation inherits from the AppCallback.AppCallbackBase
        // Make sure Kestrel Protocol is set to Http2
        // var serviceAResponse =
        //     await daprClient
        //         .InvokeMethodGrpcAsync<WeatherRequest, WeatherResponse>(
        //             "service-a",
        //             "weather.v1.WeatherService/GetWeather", request);

        // Alternative 2:
        // Make sure service is using proxy feature in Configuration
        // Make sure Kestrel Protocol is set to Http2
        // var serviceACallInvoker = DaprClient.CreateInvocationInvoker(
        //     "service-a"
        // );
        // var serviceAClient =
        //     new WeatherService.WeatherServiceClient(
        //         serviceACallInvoker
        //     );

        // Alternative 3: Make sure Kestrel Protocol is set to Http2 in service-a
        // // Hard-coded address could pass via configuration
        // using var grpcServiceAChannel =
        //     GrpcChannel.ForAddress($"http://localhost:{5291}");
        // 
        // var serviceAClient2 =
        //     new WeatherService.WeatherServiceClient(
        //         grpcServiceAChannel
        //     );
        // var serviceAResponse = await serviceAClient2.GetWeatherAsync(
        //     new WeatherRequest
        //     {
        //         City = request.City
        //     }
        // );

        // Alternative 4: Http1AndHttp2
        // var serviceAClient =
        //     new DaprClientBuilder()
        //         .UseGrpcChannelOptions(new GrpcChannelOptions
        //         {
        //             HttpVersion = Version.Parse("2.0")
        //         })
        //         .Build();
        //
        // var serviceAResponse =
        //     await serviceAClient.InvokeMethodAsync<WeatherResponse>(
        //         HttpMethod.Get,
        //         "service-a",
        //         $"v1/api/Weather/{request.City}");

        var serviceATemp = serviceAResponse?.Temperature;

        // 3. Aggregate the results (e.g., average the temperatures)
        var averageTemp = (serviceATemp + serviceBData?.Temperature ?? 0) / 2;

        return new WeatherResponse
        {
            City = request.City,
            Temperature = averageTemp
        };
    }
}