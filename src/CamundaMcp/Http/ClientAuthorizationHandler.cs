// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server.
// All Rights Reserved.

using CamundaMcp.Configuration;
using Microsoft.Extensions.Options;

namespace CamundaMcp.Http;

internal sealed class ClientAuthorizationHandler(IOptions<ClientOptions> options) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        // TODO:
        return base.SendAsync(request, cancellationToken);
    }
}
