// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server.
// All Rights Reserved.

using Aspire.Hosting.ApplicationModel;

namespace AppHost.Models;

public sealed class Credentials
{
    public required ResourceCredentials Postgres { get; init; }

    public required ResourceCredentials RabbitMq { get; init; }

    public sealed class ResourceCredentials
    {
        public required IResourceBuilder<ParameterResource> Username { get; init; }

        public required IResourceBuilder<ParameterResource> Password { get; init; }
    }
}
