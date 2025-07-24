// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server Contributors.
// All Rights Reserved.

using System.Net.Http.Headers;
using System.Text;
using Camunda.Mcp.Configuration;

namespace Camunda.Mcp.Http;

internal sealed class ClientAuthorizationHandler(IOptions<ClientOptions> options) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        string? username = options.Value.Username;

        if (string.IsNullOrWhiteSpace(username))
        {
            return base.SendAsync(request, cancellationToken);
        }

        string? password = options.Value.Password;

        string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));

        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        return base.SendAsync(request, cancellationToken);
    }
}
