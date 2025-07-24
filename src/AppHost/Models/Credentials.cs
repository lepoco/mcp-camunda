// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server Contributors.
// All Rights Reserved.

namespace AppHost.Models;

public sealed class Credentials
{
    public required ResourceCredentials Postgres { get; init; }

    public sealed class ResourceCredentials
    {
        public required IResourceBuilder<ParameterResource> Username { get; init; }

        public required IResourceBuilder<ParameterResource> Password { get; init; }
    }
}
