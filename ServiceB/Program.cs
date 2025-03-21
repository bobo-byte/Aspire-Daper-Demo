using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
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

app.Run();

// WeatherController.cs