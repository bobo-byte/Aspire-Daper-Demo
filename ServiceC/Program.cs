using ServiceC;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddGrpc(
    options =>
        options.EnableDetailedErrors = builder
            .Environment.IsDevelopment());
builder.Services.AddDaprClient();


var app = builder.Build();
app.MapGrpcService<WeatherServiceImpl>();
await app.RunAsync();