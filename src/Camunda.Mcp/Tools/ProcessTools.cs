// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server Contributors.
// All Rights Reserved.

// ReSharper disable UnusedMember.Global
namespace Camunda.Mcp.Tools;

[McpServerToolType]
internal sealed class ProcessTools
{
    [
        McpServerTool,
        Description(
            "Retrieve all BPMN process definitions available in the Camunda BPM process engine. The response includes details such as tenant, version, deployment ID, and other metadata for each process definition. Allows narrowing down results by providing parts of the process key or name, and limiting the number of results returned."
        )
    ]
    public static async Task<string> ListDefinitions(
        ICamundaService camunda,
        [Description(
            "Optional, the address of the Camunda process engine. If not provided, the default address will be used."
        )]
            string? camundaAddress,
        [Description("Optional, part of the BPMN process definiton unique key.")] string? processKeyLike,
        [Description("Optional, part of the BPMN process definition name in human readable form.")]
            string? processNameLike,
        [Description("Optional, max results to fetch provided as integer.")] int? maxResults
    )
    {
        string result = await camunda.ListProcessDefinitions(
            string.IsNullOrWhiteSpace(camundaAddress) ? null : camundaAddress,
            string.IsNullOrWhiteSpace(processKeyLike) ? null : processKeyLike,
            string.IsNullOrWhiteSpace(processNameLike) ? null : processNameLike,
            maxResults
        );

        return result;
    }

    [
        McpServerTool,
        Description(
            "Retrieve the list of active instances for a specific BPMN process definition in the Camunda process engine. The process is identified by its key. The response includes details such as tenant, key, business key, process instance id, and other metadata for each process instance."
        )
    ]
    public static async Task<string> ListInstances(
        ICamundaService camunda,
        [Description("The unique key of the BPMN process definition.")] string processKey,
        [Description("Optional, max results to fetch provided as integer.")] int? maxResults
    )
    {
        string result = await camunda.ListProcessInstancesAsync(processKey, maxResults);

        return result;
    }

    [
        McpServerTool,
        Description(
            "Count active instances for a specific BPMN process definition in the Camunda process engine. The process is identified by its key. The response includes information about the number of active instances associated with the process definition."
        )
    ]
    public static async Task<string> CountInstances(
        ICamundaService camunda,
        [Description("The unique key of the BPMN process definition.")] string processKey
    )
    {
        string result = await camunda.CountProcessInstancesAsync(processKey);

        return result;
    }

    [
        McpServerTool,
        Description(
            "Retrieve the list of variables for a specific BPMN process in the Camunda process engine. The process is identified by its process instance id or the business key. The response includes details such as tenant, key, business key, process instance id, and other metadata for each process instance."
        )
    ]
    public static async Task<string> ListVariables(
        ICamundaService camunda,
        [Description(
            "The unique process instance if of the BPMN process definition. Business key may belong to multiple processes."
        )]
            string processInstanceIdOrBusinessKey,
        [Description("Optional, max results to fetch provided as integer.")] int? maxResults
    )
    {
        string result = await camunda.ListProcessVariablesAsync(processInstanceIdOrBusinessKey, maxResults);

        return result;
    }

    [
        McpServerTool,
        Description(
            "Count variables for a specific BPMN process in the Camunda process engine. The process is identified by its process instance id or the business key. The response information about the number of variables associated with the process instance."
        )
    ]
    public static async Task<string> CountVariables(
        ICamundaService camunda,
        [Description(
            "The unique process instance if of the BPMN process definition. Business key may belong to multiple processes."
        )]
            string processInstanceIdOrBusinessKey
    )
    {
        string result = await camunda.CountProcessVariablesAsync(processInstanceIdOrBusinessKey);

        return result;
    }

    [
        McpServerTool,
        Description(
            "Requests the Cmaunda BPM process engine to resolve an incident. Resolving an incident allows the process instance to continue its execution."
        )
    ]
    public static async Task<string> ResolveIncident(
        ICamundaService camunda,
        [Description(
            "The unique identifier of the incident to resolve. This ID is typically generated by the Camunda process engine when an incident occurs."
        )]
            string incidentId
    )
    {
        string result = await camunda.ResolveIncidentAsync(incidentId);

        return result;
    }

    [
        McpServerTool,
        Description(
            "List user tasks for a specific BPMN process instance in the Camunda process engine. The process is identified by its process instance id or the business key."
        )
    ]
    public static async Task<string> ListUserTasks(
        ICamundaService camunda,
        [Description("The unique process instance id or business key of the BPMN process.")]
            string processInstanceIdOrBusinessKey,
        [Description("Optional, max results to fetch provided as integer.")] int? maxResults
    )
    {
        string result = await camunda.ListUserTasksAsync(processInstanceIdOrBusinessKey, maxResults);
        return result;
    }

    [
        McpServerTool,
        Description(
            "Count user tasks for a specific BPMN process instance in the Camunda process engine. The process is identified by its process instance id or the business key."
        )
    ]
    public static async Task<string> CountUserTasks(
        ICamundaService camunda,
        [Description("The unique process instance id or business key of the BPMN process.")]
            string processInstanceIdOrBusinessKey
    )
    {
        string result = await camunda.CountUserTasksAsync(processInstanceIdOrBusinessKey);
        return result;
    }

    [
        McpServerTool,
        Description(
            "List incidents for a specific BPMN process instance in the Camunda process engine. The process is identified by its process instance id or the business key."
        )
    ]
    public static async Task<string> ListIncidents(
        ICamundaService camunda,
        [Description("The unique process instance id or business key of the BPMN process.")]
            string processInstanceIdOrBusinessKey,
        [Description("Optional, max results to fetch provided as integer.")] int? maxResults
    )
    {
        string result = await camunda.ListIncidentsAsync(processInstanceIdOrBusinessKey, maxResults);
        return result;
    }

    [
        McpServerTool,
        Description(
            "Count incidents for a specific BPMN process instance in the Camunda process engine. The process is identified by its process instance id or the business key."
        )
    ]
    public static async Task<string> CountIncidents(
        ICamundaService camunda,
        [Description("The unique process instance id or business key of the BPMN process.")]
            string processInstanceIdOrBusinessKey
    )
    {
        string result = await camunda.CountIncidentsAsync(processInstanceIdOrBusinessKey);
        return result;
    }

    [
        McpServerTool,
        Description(
            "Complete a user task in the Camunda process engine. The task is identified by its unique task id."
        )
    ]
    public static async Task<string> CompleteUserTask(
        ICamundaService camunda,
        [Description("The unique task id of the user task to complete.")] string taskId
    )
    {
        string result = await camunda.CompleteUserTaskAsync(taskId);

        return result;
    }

    [
        McpServerTool,
        Description(
            "Send a message to a specific BPMN process instance in the Camunda process engine. The process is identified by its process instance id or the business key."
        )
    ]
    public static async Task<string> SendMessage(
        ICamundaService camunda,
        [Description("The unique process instance id or business key of the BPMN process.")]
            string processInstanceIdOrBusinessKey,
        [Description("The name of the message to send.")] string messageName
    )
    {
        string result = await camunda.SendMessageAsync(processInstanceIdOrBusinessKey, messageName);

        return result;
    }
}
