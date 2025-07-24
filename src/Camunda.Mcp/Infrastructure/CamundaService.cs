// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server Contributors.
// All Rights Reserved.

using CamundaMcp.Client;
using System.Linq;

namespace Camunda.Mcp.Infrastructure;

internal sealed class CamundaService(ICamundaClient client, ILogger<CamundaService> logger) : ICamundaService
{
    public async Task<string> ListProcessDefinitions(
        string? host,
        string? keyLike,
        string? humanReadableNameLike,
        int? maxResults,
        CancellationToken cancellationToken = default
    )
    {
        // TODO: Dynamically create clients, whether with, or without base address.
        if (Uri.TryCreate(host, UriKind.Absolute, out Uri? baseAddress))
        {
            client.Client.BaseAddress = baseAddress;
        }

        IEnumerable<ProcessDefinitionDto> processes;

        try
        {
            processes = await client.GetProcessDefinitionsAsync(
                null,
                null,
                null,
                nameLike: humanReadableNameLike,
                null,
                null,
                null,
                null,
                null,
                keyLike: keyLike,
                null,
                null,
                null,
                latestVersion: true,
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
                null,
                null,
                null,
                sortBy: SortBy31.Key,
                sortOrder: SortOrder31.Asc,
                firstResult: 0,
                maxResults: maxResults ?? 100,
                cancellationToken
            );
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get process definitions.");

            return Failure("No processes found, because: " + e.Message);
        }

        string response = string.Empty;

        foreach (ProcessDefinitionDto process in processes)
        {
            response += $"""
                    <bpmn_process>
                        <key>{process.Key}</key>
                        <deployment_id>{process.DeploymentId}</deployment_id>
                        <human_readable_name>{process.Name}</human_readable_name>
                        <resource>{process.Resource}</resource>
                        <version>{process.VersionTag}</version>
                        <tenant>{process.TenantId}</tenant>
                    </bpmn_process>
                """;
        }

        return Frame(
            $"""
            <bpmn_definitions>
                {response}
            </bpmn_definitions>
            """
        );
    }

    public async Task<string> ListProcessInstancesAsync(
        string processKey,
        int? maxResults,
        CancellationToken cancellationToken = default
    )
    {
        IEnumerable<ProcessInstanceDto>? instances;

        try
        {
            instances = await client.GetProcessInstancesAsync(
                null,
                null,
                firstResult: 0,
                maxResults: maxResults ?? 500,
                null,
                null,
                null,
                null,
                null,
                processDefinitionKey: processKey,
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
                cancellationToken: cancellationToken
            );
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get process instances for BPMN name: {BpmnName}", processKey);

            return Failure("No processes found, because: " + e.Message);
        }

        string response = string.Empty;

        foreach (ProcessInstanceDto instance in instances ?? [])
        {
            response += $"""
                    <process_instance name="{processKey}">
                        <key>{processKey}</key>
                        <business_key>{instance.BusinessKey}</business_key>
                        <process_instance_id>{instance.Id}</process_instance_id>
                        <is_suspended>{(instance.Suspended ?? false ? "yes" : "no")}</is_suspended>
                        <case_instance_id>{instance.CaseInstanceId}</case_instance_id>
                        <tenant>{instance.TenantId}</tenant>
                    </process_instance>
                """;
        }

        return Frame(
            $"""
            <bpmn_process_instances>
                {response}
            </bpmn_process_instances>
            """
        );
    }

    public async Task<string> CountProcessInstancesAsync(
        string processKey,
        CancellationToken cancellationToken = default
    )
    {
        CountResultDto count;

        try
        {
            count = await client.GetProcessInstancesCountAsync(
                null,
                null,
                null,
                null,
                null,
                processDefinitionKey: processKey,
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
                cancellationToken: cancellationToken
            );
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get process instances count for BPMN name: {BpmnName}", processKey);

            return Failure("No processes found, because: " + e.Message);
        }

        return Frame(
            $"""
            <bpmn_process_instances_count>
                {count.Count ?? 0}
            </bpmn_process_instances_count>
            """
        );
    }

    public async Task<string> ListProcessVariablesAsync(
        string processInstanceIdOrBusinessKey,
        int? maxResults,
        CancellationToken cancellationToken = default
    )
    {
        ProcessInstanceDto? process = await GetProcessInstanceAsync(
            processInstanceIdOrBusinessKey,
            cancellationToken
        );

        if (process?.Id is null)
        {
            return Failure("No process instance found for id: " + processInstanceIdOrBusinessKey);
        }

        IDictionary<string, VariableValueDto> variables;

        try
        {
            variables = await client.GetProcessInstanceVariablesAsync(process.Id, false, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching process instances.");

            return Failure("No processes found, because: " + e.Message);
        }

        string response = string.Empty;

        foreach (KeyValuePair<string, VariableValueDto> variable in variables)
        {
            response += $"""
                    <process_variable>
                        <key>{variable.Key}</key>
                        <type>{variable.Value.Type}</type>
                        <type>{variable.Value.Value}</type>
                    </process_variable>
                """;
        }

        return Frame(
            $"""
            <bpmn_process_variables>
                {response}
            </bpmn_process_variables>
            """
        );
    }

    public async Task<string> CountProcessVariablesAsync(
        string processInstanceIdOrBusinessKey,
        CancellationToken cancellationToken = default
    )
    {
        ProcessInstanceDto? process = await GetProcessInstanceAsync(
            processInstanceIdOrBusinessKey,
            cancellationToken
        );

        if (process?.Id is null)
        {
            return Failure("No process instance found for id: " + processInstanceIdOrBusinessKey);
        }

        IDictionary<string, VariableValueDto> variables;

        try
        {
            //First try to fetch the by process  its instance id
            variables = await client.GetProcessInstanceVariablesAsync(process.Id, false, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching process instances.");

            return Failure("No processes found, because: " + e.Message);
        }

        return Frame(
            $"""
            <bpmn_process_instances_count>
                {variables.Count}
            </bpmn_process_instances_count>
            """
        );
    }

    public async Task<string> ListUserTasksAsync(
        string processInstanceIdOrBusinessKey,
        int? maxResults,
        CancellationToken cancellationToken = default
    )
    {
        ProcessInstanceDto? process = await GetProcessInstanceAsync(
            processInstanceIdOrBusinessKey,
            cancellationToken
        );

        if (process?.Id is null)
        {
            return Failure("No process instance found for id: " + processInstanceIdOrBusinessKey);
        }

        IEnumerable<TaskWithAttachmentAndCommentDto> tasks;

        try
        {
            tasks = await client.GetTasksAsync(
                null,
                null,
                processInstanceId: process.Id,
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
                null,
                null,
                SortBy34.Created,
                SortOrder34.Desc,
                firstResult: 0,
                maxResults: maxResults ?? 100,
                cancellationToken
            );
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching process tasks.");

            return Failure("No tasks found, because: " + e.Message);
        }

        string response = string.Empty;

        foreach (TaskWithAttachmentAndCommentDto task in tasks)
        {
            response += $"""
                    <process_task>
                        <process_task_id>{task.Id}</process_task_id>
                        <form_key>{task.FormKey}</form_key>
                        <name>{task.Name}</name>
                        <description>{task.Description:u}</description>
                        <created_at>{task.Created:u}</created_at>
                        <due_date>{task.Due:u}</due_date>
                        <assignee>{task.Assignee}</assignee>
                        <is_suspended>{(task.Suspended ?? false ? "yes" : "no")}</is_suspended>
                        <tenant>{task.TenantId}</tenant>
                    </process_task>
                """;
        }

        return Frame(
            $"""
            <bpmn_process_tasks>
                {response}
            </bpmn_process_tasks>
            """
        );
    }

    public async Task<string> CountUserTasksAsync(
        string processInstanceIdOrBusinessKey,
        CancellationToken cancellationToken = default
    )
    {
        ProcessInstanceDto? process = await GetProcessInstanceAsync(
            processInstanceIdOrBusinessKey,
            cancellationToken
        );

        if (process?.Id is null)
        {
            return Failure("No process instance found for id: " + processInstanceIdOrBusinessKey);
        }

        CountResultDto count;

        try
        {
            count = await client.GetTasksCountAsync(
                null,
                null,
                processInstanceId: process.Id,
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
                null,
                null,
                cancellationToken
            );
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurred while fetching process tasks count.");

            return Failure("No tasks found, because: " + e.Message);
        }

        return Frame(
            $"""
            <bpmn_process_tasks>
                <tasks_count>{count.Count ?? 0}</tasks_count>
            </bpmn_process_tasks>
            """
        );
    }

    public async Task<string> ListIncidentsAsync(
        string processInstanceIdOrBusinessKey,
        int? maxResults,
        CancellationToken cancellationToken = default
    )
    {
        ProcessInstanceDto? process = await GetProcessInstanceAsync(
            processInstanceIdOrBusinessKey,
            cancellationToken
        );

        if (process?.Id is null)
        {
            return Failure("No process instance found for id: " + processInstanceIdOrBusinessKey);
        }

        IEnumerable<IncidentDto> incidents;

        try
        {
            incidents = await client.GetIncidentsAsync(
                null,
                null,
                null,
                null,
                null,
                null,
                processInstanceId: process.Id,
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
                firstResult: 0,
                maxResults: 100,
                cancellationToken
            );
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "Failed to get incidents for process instance id: {ProcessInstanceIdOrBusinessKey}",
                processInstanceIdOrBusinessKey
            );

            return Failure(
                "Failed to get incidents for process instance id: "
                    + processInstanceIdOrBusinessKey
                    + ", because: "
                    + e.Message
            );
        }

        string response = string.Empty;

        foreach (IncidentDto incident in incidents)
        {
            response += $"""
                    <process_incident>
                        <incident_id>{incident.Id}</incident_id>
                        <activity_id>{incident.ActivityId}</activity_id>
                        <execution_id>{incident.ExecutionId}</execution_id>
                        <failed_activity_id>{incident.FailedActivityId}</failed_activity_id>
                        <incident_type>{incident.IncidentType}</incident_type>
                        <incident_message>{incident.IncidentMessage}</incident_message>
                        <incident_timestamp>{incident.IncidentTimestamp:u}</incident_timestamp>
                        <annotation>{incident.Annotation}</annotation>
                        <tenant>{incident.TenantId}</tenant>
                    </process_incident>
                """;
        }

        return Frame(
            $"""
            <bpmn_process_incidents>
                {response}
            </bpmn_process_incidents>
            """
        );
    }

    public async Task<string> CountIncidentsAsync(
        string processInstanceIdOrBusinessKey,
        CancellationToken cancellationToken = default
    )
    {
        ProcessInstanceDto? process = await GetProcessInstanceAsync(
            processInstanceIdOrBusinessKey,
            cancellationToken
        );

        if (process?.Id is null)
        {
            return Failure("No process instance found for id: " + processInstanceIdOrBusinessKey);
        }

        CountResultDto count;

        try
        {
            count = await client.GetIncidentsCountAsync(
                null,
                null,
                null,
                null,
                null,
                null,
                processInstanceId: process.Id,
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
                cancellationToken
            );
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "Failed to get incidents for process instance id: {ProcessInstanceIdOrBusinessKey}",
                processInstanceIdOrBusinessKey
            );

            return Failure(
                "Failed to get incidents for process instance id: "
                    + processInstanceIdOrBusinessKey
                    + ", because: "
                    + e.Message
            );
        }

        return Frame(
            $"""
            <bpmn_process_incidents>
                <incident_count>{count.Count ?? 0}</incident_count>
            </bpmn_process_incidents>
            """
        );
    }

    public async Task<string> ResolveIncidentAsync(
        string incidentId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            IncidentDto incident = await client.GetIncidentAsync(incidentId, cancellationToken);

            if (
                incident.IncidentType?.Contains("externaltask", StringComparison.InvariantCultureIgnoreCase)
                ?? false
            )
            {
                await client.SetExternalTaskRetriesAsync(
                    new SetRetriesForExternalTasksDto
                    {
                        ProcessInstanceIds = incident.ProcessInstanceId is null
                            ? null
                            : [incident.ProcessInstanceId],
                        ExternalTaskIds = incident.Configuration is null ? null : [incident.Configuration],
                        Retries = 1,
                    },
                    cancellationToken
                );
            }
            else if (
                incident.IncidentType?.Contains("failedjob", StringComparison.InvariantCultureIgnoreCase)
                ?? false
            )
            {
                await client.SetJobRetriesAsync(
                    incidentId,
                    new JobRetriesDto { Retries = 1 },
                    cancellationToken
                );
            }
            else
            {
                await client.ResolveIncidentAsync(incidentId, cancellationToken);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to resolve incident with id: {IncidentId}", incidentId);

            return Failure("Failed to resolve incident, because: " + e.Message);
        }

        return Frame(
            $"""
            <incident_resolved>
                <incident_id>{incidentId}</incident_id>
            </incident_resolved>
            """
        );
    }

    public async Task<string> CompleteUserTaskAsync(
        string taskId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            await client.CompleteAsync(
                taskId,
                new CompleteTaskDto { AdditionalProperties = new Dictionary<string, object>() },
                cancellationToken
            );
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to complete user task with id: {TaskId}", taskId);
        }

        return Frame(
            $"""
            <user_task_completed>
                <task_id>{taskId}</task_id>
            </user_task_completed>
            """
        );
    }

    public async Task<string> SendMessageAsync(
        string processInstanceIdOrBusinessKey,
        string messageName,
        CancellationToken cancellationToken = default
    )
    {
        ProcessInstanceDto? process = await GetProcessInstanceAsync(
            processInstanceIdOrBusinessKey,
            cancellationToken
        );

        if (process?.Id is null)
        {
            return Failure("No process instance found for id: " + processInstanceIdOrBusinessKey);
        }

        try
        {
            await client.DeliverMessageAsync(
                new CorrelationMessageDto { ProcessInstanceId = process.Id, MessageName = messageName, ResultEnabled = true },
                cancellationToken
            );
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "Failed to send message {MessageName} to process instance id: {ProcessInstanceIdOrBusinessKey}",
                messageName,
                processInstanceIdOrBusinessKey
            );

            return Failure("Failed to send message, because: " + e.Message);
        }

        return Frame(
            $"""
            <message_sent>
                <process_instance_id>{process.Id}</process_instance_id>
                <message_name>{messageName}</message_name>
            </message_sent>
            """
        );
    }

    private async Task<ProcessInstanceDto?> GetProcessInstanceAsync(
        string processInstanceIdOrBusinessKey,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            ProcessInstanceDto processInstance = await client.GetProcessInstanceAsync(
                processInstanceIdOrBusinessKey,
                cancellationToken
            );

            return processInstance;
        }
        catch (Exception e)
        {
            logger.LogWarning(
                e,
                "Failed to get process instance for id: {ProcessInstanceIdOrBusinessKey}",
                processInstanceIdOrBusinessKey
            );
        }

        try
        {
            IEnumerable<ProcessInstanceDto> instances = await client.GetProcessInstancesAsync(
                sortBy: SortBy32.DefinitionKey,
                sortOrder: SortOrder32.Asc,
                firstResult: 0,
                maxResults: 2,
                null,
                businessKey: processInstanceIdOrBusinessKey,
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
                cancellationToken: cancellationToken
            );

            ProcessInstanceDto[] enumeratedInstances = instances.ToArray();

            if (enumeratedInstances.Length < 1)
            {
                return null;
            }

            if (enumeratedInstances.Length > 1)
            {
                logger.LogWarning(
                    "Found mre than one process instance for business key: {ProcessInstanceIdOrBusinessKey}",
                    processInstanceIdOrBusinessKey
                );
            }

            string? processId = enumeratedInstances.First().Id;

            if (processId is null)
            {
                logger.LogError(
                    "Process instance id is null for business key: {ProcessInstanceIdOrBusinessKey}",
                    processInstanceIdOrBusinessKey
                );

                return null;
            }

            ProcessInstanceDto processInstance = await client.GetProcessInstanceAsync(
                processId,
                cancellationToken
            );

            return processInstance;
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "Failed to get process instances for business key: {ProcessInstanceIdOrBusinessKey}",
                processInstanceIdOrBusinessKey
            );

            return null;
        }
    }

    private static string Failure(string message)
    {
        return Frame($"<operation_failed>{message}</operation_failed>");
    }

    private static string Frame(string response)
    {
        return $"""
            The following is a response from the Camunda MCP server. The data is formatted as pseudo XML (inside <> brackets) for easier parsing and manipulation.
            <mcp_server_response> is the root element of the response. <metadata> is additional data provided by the server, such as the timestamp of the response. <response> contains the actual response data from the server. Inside <response>, you will find the data requested by the client or error inside <operation_failed> if the operation failed.

            Start of the response:
            <mcp_server_response>
                <metadata>
                    <timestamp>{DateTime.UtcNow:O}</timestamp>
                </metadata>
                <response>
                    {response}
                </response>
            </mcp_server_response>
            """;
    }
}
