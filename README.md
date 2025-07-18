# Camunda MCP Server

[Created with ❤ in Poland by lepo.co](https://lepo.co/) and [wonderful open-source community](https://github.com/lepoco/mcp-camunda/graphs/contributors)  
A Model Context Protocol (MCP) server for Camunda.

Provides access to your Camunda instance and the surrounding ecosystem.

## Features

The following features are currently available in the MCP server. This list is for informational purposes only and does not represent a roadmap or commitment to future features.

### Process Definitions

- **List process definitions**: Retrieve all process definitions deployed in Camunda.
- **Get process definition by ID**: Fetch details of a specific process definition using its unique identifier.
- **Start a process instance**: Start a new instance of a process definition.

### Process Instances

- **List process instances**: Retrieve all active process instances.
- **Get process instance by ID**: Fetch details of a specific process instance.
- **Terminate a process instance**: End a running process instance.

### Tasks

- **List tasks**: Retrieve all tasks available for processing.
- **Get task by ID**: Fetch details of a specific task.
- **Complete a task**: Mark a task as completed and move the process forward.

### Variables

- **Get process variables**: Retrieve variables associated with a process instance.
- **Set process variables**: Update or create variables for a process instance.

### Incidents

- **List incidents**: Retrieve all incidents in the Camunda engine.
- **Resolve an incident**: Mark an incident as resolved.

## Tools

| Tool                         | Category            | Description                                    |
| ---------------------------- | ------------------- | ---------------------------------------------- |
| `list_process_definitions`   | Process Definitions | List all process definitions.                  |
| `get_process_definition`     | Process Definitions | Get a process definition by ID.                |
| `start_process_instance`     | Process Definitions | Start a new process instance.                  |
| `list_process_instances`     | Process Instances   | List all active process instances.             |
| `get_process_instance`       | Process Instances   | Get a process instance by ID.                  |
| `terminate_process_instance` | Process Instances   | Terminate a running process instance.          |
| `list_tasks`                 | Tasks               | List all tasks.                                |
| `get_task`                   | Tasks               | Get a task by ID.                              |
| `complete_task`              | Tasks               | Complete a task.                               |
| `get_process_variables`      | Variables           | Get variables for a process instance.          |
| `set_process_variables`      | Variables           | Set variables for a process instance.          |
| `list_incidents`             | Incidents           | List all incidents.                            |
| `resolve_incident`           | Incidents           | Resolve an incident.                           |
| `query_historical_data`      | History             | Query historical data for processes and tasks. |

## Copyright

Camunda trademarks are trademarks of Camunda Services GmbH and belong to Camunda Services GmbH.
https://camunda.com/
