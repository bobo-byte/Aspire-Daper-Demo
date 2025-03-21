using CommunityToolkit.Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);
// Dapr components
var stateStore = builder.AddDaprStateStore("statestore");
var pubSub = builder.AddDaprPubSub("pubsub");

// ServiceA (HTTP + gRPC)
builder.AddProject<Projects.ServiceA>("service-a")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "service-a",
        AppProtocol = "grpc", // Default protocol, HTTP also supported
        DaprGrpcPort = 50001,
        DaprHttpPort = 3500,
        EnableApiLogging = true
    })
    .WithReference(stateStore)
    .WithReference(pubSub);

// ServiceB (HTTP-only)
builder.AddProject<Projects.ServiceB>("service-b")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "service-b",
        AppProtocol = "http",
        DaprHttpPort = 3501,
        EnableApiLogging = true
    })
    .WithReference(pubSub);

// ServiceC (gRPC-only)
builder.AddProject<Projects.ServiceC>("service-c")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "service-c",
        AppProtocol = "grpc",
        DaprGrpcPort = 50002,
        EnableApiLogging = true
    })
    .WithReference(stateStore);

builder.Build().Run();