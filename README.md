# 🔄 Camunda MCP Server

_Your Agentic Gateway to Camunda BPM._

[Created with ❤ in Poland by lepo.co](https://lepo.co/) and [the awesome open-source community](https://github.com/lepoco/mcp-camunda/graphs/contributors).  
This repository provides a powerful set of tools to interact with the Camunda 7 Community Edition Engine using the Model Context Protocol (MCP). Whether you're automating workflows, querying process instances, or integrating with external systems, Camunda MCP Server is your agentic solution for seamless interaction with Camunda.

### 🛠️ Available Server Tools

| Tool               | Description                                                 |
| ------------------ | ----------------------------------------------------------- |
| `list_definitions` | List deployed Camunda definitions.                          |
| `list_instances`   | List active instances of selected Camunda process.          |
| `count_instances`  | Count active instances of selected Camunda process.         |
| `list_variables`   | List variables of the selected Camunda process.             |
| `count_variables`  | Count variables of the selected Camunda process.            |
| `list_incidents`   | Lists variables of the selected Camunda process.            |
| `count_incidents`  | Count variables of the selected Camunda process.            |
| `list_user_tasks`  | Lists user tasks of the selected Camunda process.           |
| `count_user_tasks` | Count user tasks of the selected Camunda process.           |
| `resolve_incident` | Requests the Camunda process engine to resolve an incident. |

### 🐳 Run the server with Docker

Build the image:

```bash
docker buildx build ./ -t mcp/camunda --no-cache
# or
dotnet publish ./src/Camunda.Mcp/Camunda.Mcp.csproj -c Release /t:PublishContainer
```

Run the container:

```bash
docker run -d -i --rm --name mcp-camunda mcp/camunda
# or for HTTP mode:
docker run -d --name mcp-camunda mcp/camunda -e MODE=Http -e CAMUNDA_HOST=http://host.docker.internal:8080/ -p 64623:8080
```

Example MCP config (`.mcp.json`):

```json
{
  "servers": {
    "camunda": {
      "type": "stdio",
      "command": "docker",
      "args": [
        "run",
        "-i",
        "--rm",
        "mcp/camunda",
        "-e",
        "CAMUNDA_HOST=http://host.docker.internal:8080/engine-rest/"
      ]
    }
  },
  "inputs": []
}
```

## Code of Conduct

This project has adopted the code of conduct defined by the Contributor Covenant to clarify expected behavior in our community.

## License

Cammunda is a registered trademark of [Camunda Services GmbH Zossener Strasse 55-58, 10961 Berlin, Germany](https://camunda.com/).

It's used in this project as an information that the tools provided here are compatible with Camunda 7 Community Edition Engine, which is an open-source project licensed under the [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0).

<https://github.com/camunda/camunda-bpm-platform>

**Camunda MCP Server** is free and open source software licensed under **MIT License**. You can use it in private and commercial projects.  
Keep in mind that you must include a copy of the license in your project.
