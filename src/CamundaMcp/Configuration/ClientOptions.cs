// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server.
// All Rights Reserved.

namespace CamundaMcp.Configuration;

public sealed class ClientOptions
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "Service URI cannot be empty.")]
    public Uri? Host { get; set; }

    [StringLength(2_000, MinimumLength = 1, ErrorMessage = "Username must be between 1 and 2000 characters.")]
    public string? Username { get; set; }

    [StringLength(2_000, MinimumLength = 1, ErrorMessage = "Password must be between 1 and 2000 characters.")]
    public string? Password { get; set; }

    [Range(1, 1_800, ErrorMessage = "Timeout must be between 1 and 1_800 seconds.")]
    public int TimeoutInSeconds { get; set; } = 120;
}
