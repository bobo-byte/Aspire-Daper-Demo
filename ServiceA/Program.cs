using ServiceA;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults(); // (logging, telemetry, etc)

// Add gRPC
builder.Services.AddGrpc(options => options.EnableDetailedErrors = true);
builder.Services.AddGrpcReflection();

// Dapr
builder.Services.AddDaprClient();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

var app = builder.Build();
// Middleware
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCloudEvents(); // Dapr pub/sub 
app.MapSubscribeHandler();

app.MapGrpcService<WeatherServiceImpl>();
app.MapGrpcReflectionService();
app.MapControllers();
app.MapGet("/health", () => Results.Ok("OK"));

app.Run();