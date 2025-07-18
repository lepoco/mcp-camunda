// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server.
// All Rights Reserved.

using CamundaMcp.Client;
using CamundaMcp.Configuration;
using CamundaMcp.Http;
using CamundaMcp.Resources;
using CamundaMcp.Tools;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});

#if DEBUG
builder
    .Services.AddOpenTelemetry()
    .WithTracing(b => b.AddSource("*").AddAspNetCoreInstrumentation().AddHttpClientInstrumentation())
    .WithMetrics(b => b.AddMeter("*").AddAspNetCoreInstrumentation().AddHttpClientInstrumentation())
    .WithLogging()
    .UseOtlpExporter();
#endif

builder
    .Services.Configure<ClientOptions>(clientOptions =>
    {
        ConfigurationManager configuration = builder.Configuration;

        IConfigurationSection section = configuration.GetSection("Camunda");
        section.Bind(clientOptions);

        string? camundaHost = Environment.GetEnvironmentVariable("CAMUNDA_HOST");
        if (!string.IsNullOrWhiteSpace(camundaHost))
        {
            clientOptions.Host = new Uri(camundaHost);
        }

        string? camundaUser = Environment.GetEnvironmentVariable("CAMUNDA_USER");
        if (!string.IsNullOrWhiteSpace(camundaUser))
        {
            clientOptions.Username = camundaUser;
        }

        string? camundaPassword = Environment.GetEnvironmentVariable("CAMUNDA_PASSWORD");
        if (!string.IsNullOrWhiteSpace(camundaPassword))
        {
            clientOptions.Password = camundaPassword;
        }
    })
    .AddOptions<ClientOptions>()
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder
    .Services.AddTransient<ClientAuthorizationHandler>()
    .AddHttpClient<ICamundaClient, CamundaClient>(
        (services, client) =>
        {
            IOptions<ClientOptions> options = services.GetRequiredService<IOptions<ClientOptions>>();

            client.BaseAddress = options.Value.Host;
            client.Timeout = TimeSpan.FromSeconds(options.Value.TimeoutInSeconds);
        }
    )
    .AddHttpMessageHandler<ClientAuthorizationHandler>();

ServerOptions serverOptions = new();
IConfigurationSection section = builder.Configuration.GetSection("Server");
section.Bind(serverOptions);

string? mode = Environment.GetEnvironmentVariable("mode");
if (mode?.Contains("http", StringComparison.InvariantCultureIgnoreCase) ?? false)
{
    serverOptions.Mode = McpMode.Http;
}

IMcpServerBuilder mcpBuilder = builder.Services.AddMcpServer();

if (serverOptions.Mode == McpMode.Stdio)
{
    _ = mcpBuilder.WithStdioServerTransport();
}
else
{
    _ = mcpBuilder.WithHttpTransport();
}

_ = mcpBuilder.WithTools<ProcessTools>().WithResources<ProcessResources>();

await using WebApplication app = builder.Build();

if (serverOptions.Mode == McpMode.Http)
{
    app.MapMcp();
}

await app.RunAsync();

return;
