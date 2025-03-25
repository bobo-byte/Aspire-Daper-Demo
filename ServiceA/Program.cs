using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using ServiceA;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Solution 2:
// Add separate endpoints for handling Http (Controller based api) and Http 2 (grpc)
// aspire uses the launchSettings.json to orchestrate the kestrel server endpoints -
// - by default this will throw an error when the matching the same ports as the launchSettings 
// - a solution might be to state that --no-launch-profile as argument on the Aspire sidecar
// - alternatively set them to be different, the only thing is that it will not show the endpoint on the Aspire dashboard
// builder.WebHost.UseKestrel(options =>
// {
//     // port 8282 for HTTP/2 (unencrypted)
//     options.ListenLocalhost(5921,
//         listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
//     // port 80 for k8s probe which is too stupid to understand clear HTTP/2 - #97330
//     options.ListenLocalhost(5922,
//         listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
// });

// Current solution try to use gRPC JSON Transcoding to allow regular rest requests to be sent over gRPC protocol with the support of .proto files

builder.AddServiceDefaults();
builder.Services.AddGrpc(
        options =>
            options.EnableDetailedErrors = builder
                .Environment.IsDevelopment())
    .AddJsonTranscoding();

builder.Services.AddDaprClient();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
            { Title = "gRPC JSON transcoding example", Version = "v1" });

    // Add <GenerateDocumentationFile>true</GenerateDocumentationFile> if this is needed.

    // var filePath = Path.Combine(AppContext.BaseDirectory, "Server.xml");
    // c.IncludeXmlComments(filePath);
    // c.IncludeGrpcXmlComments(filePath, includeControllerXmlComments: true);
});
builder.Services.AddGrpcSwagger();

//Solution 2
// builder.Services.AddGrpcReflection(); // Might not be needed
// builder.Services.AddControllers();

var app = builder.Build();

// Solution 2: 
// Middleware
//app.UseRouting();
//app.UseSwagger();
//app.UseSwaggerUI();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json",
        "gRPC JSON transcoding example V1");
});
app.UseCloudEvents(); // Dapr pub/sub
app.MapSubscribeHandler();

app.MapGrpcService<WeatherServiceImpl>();

// Solution 2: 
// app.MapGrpcReflectionService(); // Might not be needed
// app.MapControllers();

await app.RunAsync();