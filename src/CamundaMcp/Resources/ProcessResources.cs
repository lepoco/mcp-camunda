// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server.
// All Rights Reserved.

namespace CamundaMcp.Resources;

[McpServerResourceType]
public class ProcessResources
{
    [McpServerResource, Description("A direct text resource")]
    public static string DirectTextResource() => "This is a direct resource";
}