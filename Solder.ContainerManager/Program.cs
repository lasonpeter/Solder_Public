using Docker.DotNet;
using FastEndpoints;
using Scalar.AspNetCore;
using Solder.ContainerManager;
using Solder.ContainerManager.Core;
using Solder.ContainerManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- OpenAPI ---
builder.Services.AddOpenApi();

// --- Docker SDK ---
var dockerUri = OperatingSystem.IsWindows()
    ? new Uri("npipe://./pipe/docker_engine")
    : new Uri("unix:///var/run/docker.sock");
builder.Services.AddSingleton<IDockerClient>(new DockerClientConfiguration(dockerUri).CreateClient());

// --- Clean Architecture Services ---
builder.Services.AddScoped<IContainerService, DockerContainerService>();

// --- FastEndpoints ---
builder.Services.AddFastEndpoints(o => { o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All; });

var app = builder.Build();

// --- Middleware Pipeline ---
app.UseFastEndpoints();

// Only enable the API UI in development to keep the binary slim
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Serves the openapi.json
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Solder Container Manager API")
            .WithTheme(ScalarTheme.Moon);
    });
}

app.Run();