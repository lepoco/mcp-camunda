// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server Contributors.
// All Rights Reserved.

using Camunda.Mcp.Configuration;

namespace Camunda.Mcp.Infrastructure;

internal sealed class ResolveByPingService(
    IHttpClientFactory clientFactory,
    IOptionsMonitor<ClientOptions> clientOptions,
    ILogger<ResolveByPingService> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (clientOptions.CurrentValue.Host is null)
        {
            // NOTE: Get uri at runtime
            return;
        }

        using HttpClient client = clientFactory.CreateClient(nameof(ResolveByPingService));

        bool defaultPath = await PingAsync(client, cancellationToken: cancellationToken);

        if (defaultPath)
        {
            return;
        }

        bool engineRestPath = await PingAsync(client, "/engine-rest", cancellationToken: cancellationToken);

        if (engineRestPath)
        {
            string newUrl = clientOptions.CurrentValue.Host.TrimEnd('/') + "/engine-rest/";

            clientOptions.CurrentValue.Host = newUrl;

            logger.LogInformation(
                "Camunda server at \"{Url}\" is using the /engine-rest path. Updated client options.",
                newUrl
            );
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task<bool> PingAsync(
        HttpClient client,
        string? subPath = null,
        CancellationToken cancellationToken = default
    )
    {
        string basePath = clientOptions.CurrentValue.Host?.TrimEnd('/') + subPath + "/authorization";

        try
        {
            using HttpResponseMessage response = await client.SendAsync(
                new HttpRequestMessage
                {
                    Method = HttpMethod.Options,
                    RequestUri = new Uri(basePath, UriKind.Absolute),
                },
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
            );

            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            logger.LogWarning(e, " Failed to ping Camunda server at {Url}.", basePath);
        }

        return false;
    }
}
