// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server.
// All Rights Reserved.

using CamundaMcp.Client;

namespace CamundaMcp.Tools;

[McpServerToolType]
// ReSharper disable UnusedMember.Global
public sealed class ProcessTools
{
    [McpServerTool, Description("List instances of the selected BPMN in Camunda process engine.")]
    public static async Task<long> ListProcessInstances(ICamundaClient client, [Description("Name or id of the BPMN process")] string bpmnName)
    {
        using CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(120));

        CountResultDto? count = await client.GetProcessInstancesCountAsync(
            null,
            null,
            null,
            null,
            bpmnName,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            true,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            cancellationToken: cancellationTokenSource.Token
        );

        return count?.Count ?? 0;
    }

    [McpServerTool, Description("List all process definitions in Camunda process engine.")]
    public static async Task<IEnumerable<string>> ListProcessDefinitions(ICamundaClient client)
    {
        await Task.CompletedTask;

        //using CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(120));
        //IEnumerable<ProcessDefinitionDto>? definitions = await client.GetProcessDefinitionsAsync(
        //    cancellationToken: cancellationTokenSource.Token
        //);

        //if (definitions is null)
        //{
        //    return [];
        //}

        //return definitions.Select(x => x.Name ?? "unknown");

        return [];
    }
}

//    [McpServerTool, Description("Echoes the message back to the client.")]
//    public static string Echo(string message) => $"Hello from C#: {message}";

//    //[McpServerTool, Description("Echoes in reverse the message sent by the client.")]
//    //public static string ReverseEcho(string message) => new string(message.Reverse().ToArray());

//    //[McpServerTool, Description("List all process definitions.")]
//    //public static async Task<IEnumerable<string>> ListProcessDefinitions(ICamundaClient client)
//    //{
//    //    //using CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(120));
//    //    //IEnumerable<ProcessDefinitionDto>? definitions = await client.GetProcessDefinitionsAsync(
//    //    //    cancellationToken: cancellationTokenSource.Token
//    //    //);

//    //    //if (definitions is null)
//    //    //{
//    //    //    return [];
//    //    //}

//    //    //return definitions.Select(x => x.Name ?? "unknown");

//    //    return [];
//    //}

//    //[McpServerTool, Description("Get a process definition by ID.")]
//    //public static string GetProcessDefinition(string id) => string.Empty;

//    //[McpServerTool, Description("Start a new process instance.")]
//    //public static string StartProcessInstance(string processDefinitionId) => string.Empty;

//    //[McpServerTool, Description("List all active process instances.")]
//    //public static List<string> ListProcessInstances() => new List<string>();

//    //[McpServerTool, Description("Get a process instance by ID.")]
//    //public static string GetProcessInstance(string id) => string.Empty;

//    //[McpServerTool, Description("Terminate a running process instance.")]
//    //public static bool TerminateProcessInstance(string id) => true;

//    //[McpServerTool, Description("List all tasks.")]
//    //public static List<string> ListTasks() => new List<string>();

//    //[McpServerTool, Description("Get a task by ID.")]
//    //public static string GetTask(string id) => string.Empty;

//    //[McpServerTool, Description("Complete a task.")]
//    //public static bool CompleteTask(string id) => true;

//    //[McpServerTool, Description("Get variables for a process instance.")]
//    //public static Dictionary<string, object> GetProcessVariables(string processInstanceId) =>
//    //    new Dictionary<string, object>();

//    //[McpServerTool, Description("Set variables for a process instance.")]
//    //public static bool SetProcessVariables(string processInstanceId, Dictionary<string, object> variables) =>
//    //    true;

//    //[McpServerTool, Description("List all incidents.")]
//    //public static List<string> ListIncidents() => new List<string>();

//    //[McpServerTool, Description("Resolve an incident.")]
//    //public static bool ResolveIncident(string id) => true;

//    //[McpServerTool, Description("Query historical data for processes and tasks.")]
//    //public static List<string> QueryHistoricalData(string query) => new List<string>();