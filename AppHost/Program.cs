using CommunityToolkit.Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);
// Dapr components
var stateStore = builder.AddDaprStateStore("statestore");
var pubSub = builder.AddDaprPubSub("pubsub");

var daprSideCarLogLevel = builder.Environment.EnvironmentName == "Development"
    ? "debug"
    : "info";

// ServiceA (HTTP + gRPC)
builder.AddProject<Projects.ServiceA>("service-a")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "service-a",
        AppPort = 5291,
        DaprGrpcPort = 50001,
        DaprHttpPort = 3500,
        Config = Path.Combine("..", "ServiceA", "dapr", "config.yml"),
        EnableApiLogging = true,
        LogLevel = daprSideCarLogLevel
    })
    .WithReference(stateStore)
    .WithReference(pubSub);

// ServiceB (HTTP-only)
builder.AddProject<Projects.ServiceB>("service-b")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "service-b",
        AppPort = 5098,
        AppProtocol = "http",
        DaprHttpPort = 3501,
        EnableApiLogging = true,
        LogLevel = daprSideCarLogLevel
    })
    .WithReference(pubSub);

// ServiceC (gRPC-only)
builder.AddProject<Projects.ServiceC>("service-c")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "service-c",
        AppPort = 5144,
        AppProtocol = "grpc",
        DaprGrpcPort = 50002,
        EnableApiLogging = true,
        LogLevel = daprSideCarLogLevel
    })
    .WithReference(stateStore);

builder.Build().Run();