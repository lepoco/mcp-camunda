// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server Contributors.
// All Rights Reserved.

namespace Camunda.Mcp;

internal interface ICamundaService
{
    Task<string> ListProcessDefinitions(
        string? host,
        string? keyLike,
        string? humanReadableNameLike,
        int? maxResults,
        CancellationToken cancellationToken = default
    );

    Task<string> ListProcessInstancesAsync(
        string processKey,
        int? maxResults,
        CancellationToken cancellationToken = default
    );

    Task<string> CountProcessInstancesAsync(string processKey, CancellationToken cancellationToken = default);

    Task<string> ListProcessVariablesAsync(
        string processInstanceIdOrBusinessKey,
        int? maxResults,
        CancellationToken cancellationToken = default
    );

    Task<string> CountProcessVariablesAsync(
        string processInstanceIdOrBusinessKey,
        CancellationToken cancellationToken = default
    );

    Task<string> ListUserTasksAsync(
        string processInstanceIdOrBusinessKey,
        int? maxResults,
        CancellationToken cancellationToken = default
    );

    Task<string> CountUserTasksAsync(
        string processInstanceIdOrBusinessKey,
        CancellationToken cancellationToken = default
    );

    Task<string> ListIncidentsAsync(
        string processInstanceIdOrBusinessKey,
        int? maxResults,
        CancellationToken cancellationToken = default
    );

    Task<string> CountIncidentsAsync(
        string processInstanceIdOrBusinessKey,
        CancellationToken cancellationToken = default
    );

    Task<string> ResolveIncidentAsync(string incidentId, CancellationToken cancellationToken = default);

    Task<string> CompleteUserTaskAsync(string taskId, CancellationToken cancellationToken = default);

    Task<string> SendMessageAsync(
        string processInstanceIdOrBusinessKey,
        string messageName,
        CancellationToken cancellationToken = default
    );
}
