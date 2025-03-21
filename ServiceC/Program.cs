using ServiceC;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddGrpc(options => options.EnableDetailedErrors = true);
builder.Services.AddGrpcReflection();
builder.Services.AddDaprClient();


var app = builder.Build();
app.MapGrpcService<WeatherServiceImpl>();
app.MapGrpcReflectionService();
app.Run();