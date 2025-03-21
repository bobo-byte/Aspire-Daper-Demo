using ServiceA;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(); // (logging, telemetry, etc)

// Add gRPC
builder.Services.AddGrpc();

// Dapr
builder.Services.AddDaprClient();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

var app = builder.Build();
// Middleware
app.UseRouting();
app.UseSwagger();
app.UseCloudEvents(); // Dapr pub/sub 
app.MapSubscribeHandler();

app.MapGrpcService<WeatherServiceImpl>();
app.MapControllers();
app.MapGet("/health", () => Results.Ok("OK"));

app.Run();