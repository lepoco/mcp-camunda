// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server Contributors.
// All Rights Reserved.

using Camunda.Mcp;
using Camunda.Mcp.Configuration;
using Camunda.Mcp.Http;
using Camunda.Mcp.Infrastructure;
using Camunda.Mcp.Logging;
using Camunda.Mcp.Tools;
using CamundaMcp.Client;

ILogger logger = StaticLoggerFactory.New();
ServerOptions serverOptions = ConfigurationFactory.CreateServerOptions(args);

logger.LogInformation("Configuring MCP Server with mode: {Mode}", serverOptions.Mode);

// NOTE: Do not use whole ASP.NET stack with just interactive console
if (serverOptions.Mode == McpMode.Stdio)
{
    logger.LogInformation("Adding Stdio transport to MCP Server.");

    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
    AddMcpServices(builder, serverOptions.Mode).WithStdioServerTransport();

    logger.LogInformation("Starting the application.");

    await builder.Build().RunAsync();

    // NOTE: Stop here, as we are running in Stdio mode.
    return;
}

logger.LogInformation("Adding HTTP transport to MCP Server.");

WebApplicationBuilder webBuilder = WebApplication.CreateBuilder(args);
IMcpServerBuilder mcpBuilder = AddMcpServices(webBuilder, serverOptions.Mode).WithHttpTransport();

if (serverOptions.Mode == McpMode.Both)
{
    logger.LogInformation("Adding Stdio transport to MCP Server.");
    _ = mcpBuilder.WithStdioServerTransport();
}

#if !DEBUG
webBuilder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
    options.ListenAnyIP(8080);
    options.ListenAnyIP(8000);
});
#endif

await using WebApplication app = webBuilder.Build();

app.MapMcp();
app.MapHealthChecks("/healthz");

await app.RunAsync();

return;

static IMcpServerBuilder AddMcpServices(IHostApplicationBuilder builder, McpMode mode)
{
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
            IConfigurationManager configuration = builder.Configuration;
            configuration.GetSection("Camunda").Bind(clientOptions);

            string? camundaHost = builder.Configuration["CAMUNDA_HOST"];
            if (!string.IsNullOrWhiteSpace(camundaHost))
            {
                clientOptions.Host = camundaHost;
            }

            string? camundaUser = builder.Configuration["CAMUNDA_USER"];
            if (!string.IsNullOrWhiteSpace(camundaUser))
            {
                clientOptions.Username = camundaUser;
            }

            string? camundaPassword = builder.Configuration["CAMUNDA_PASSWORD"];
            if (!string.IsNullOrWhiteSpace(camundaPassword))
            {
                clientOptions.Password = camundaPassword;
            }

            StaticLoggerFactory.New().LogInformation("Camunda Client host: {Options}", clientOptions.Host);
        })
        .AddOptions<ClientOptions>()
        .ValidateDataAnnotations()
        .ValidateOnStart();

    builder.Services.AddTransient<ICamundaService, CamundaService>();

    builder
        .Services.AddHttpClient(nameof(ResolveByPingService))
        .ConfigurePrimaryHttpMessageHandler(() =>
            new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                AllowAutoRedirect = true,
            }
        );

    builder
        .Services.AddTransient<ClientAuthorizationHandler>()
        .AddHttpClient<ICamundaClient, CamundaClient>(
            (provider, client) =>
            {
                IOptionsMonitor<ClientOptions> options = provider.GetRequiredService<
                    IOptionsMonitor<ClientOptions>
                >();

                client.BaseAddress = !string.IsNullOrWhiteSpace(options.CurrentValue.Host)
                    ? new Uri(options.CurrentValue.Host, UriKind.Absolute)
                    : new Uri("http://127.0.0.1:8080/engine-rest/", UriKind.Absolute);
            }
        )
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                AllowAutoRedirect = true,
            };
        })
        .AddHttpMessageHandler<ClientAuthorizationHandler>();

    builder.Services.AddHostedService<ResolveByPingService>();

    if (mode != McpMode.Stdio)
    {
        builder.Services.AddHealthChecks();
    }

    IMcpServerBuilder mcpBuilder = builder
        .Services.AddMcpServer(mcp =>
        {
            mcp.ServerInfo = new Implementation { Name = "Camunda BPM MCP Server", Version = "1.0.0" };
            mcp.ScopeRequests = true;
        })
        .WithTools<ProcessTools>();

    return mcpBuilder;
}
