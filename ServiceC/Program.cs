using Grpc.Net.Client;
using ServiceC;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.IgnoreUnknownServices = true;
});
builder.Services.AddGrpcReflection();
builder.Services.AddDaprClient(configure =>
{
    configure.UseGrpcChannelOptions(
        new GrpcChannelOptions
        {
            UnsafeUseInsecureChannelCallCredentials =
                builder.Environment.IsDevelopment()
        }
    );
});


var app = builder.Build();
app.MapGrpcService<WeatherServiceImpl>();
app.MapGrpcReflectionService();
app.Run();