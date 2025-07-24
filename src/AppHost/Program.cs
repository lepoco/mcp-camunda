// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server Contributors.
// All Rights Reserved.

using AppHost.Extensions;
using AppHost.Models;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

Credentials credentials = builder.GetCredentials();

builder.AddForwardedHeaders();

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("postgres", userName: credentials.Postgres.Username, password: credentials.Postgres.Password)
    .WithImage("postgres")
    .WithEndpoint(name: "database", scheme: "tcp", port: null, targetPort: 5432, isProxied: false)
    .WithArgs("-c", "max_connections=1000")
    .WithLifetime(ContainerLifetime.Persistent);

if (!builder.Environment.IsProduction())
{
    postgres.WithPgAdmin(c => c.WithLifetime(ContainerLifetime.Persistent));
}

IResourceBuilder<ContainerResource> camunda = builder
    .AddContainer("camunda", "camunda/camunda-bpm-platform", "run-7.23.0")
    .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "dashboard")
    .WithArgs("./camunda.sh", "--rest", "--webapps")
    .WithEnvironment("DB_DRIVER", "org.postgresql.Driver")
    .WithEnvironment(async context =>
    {
        ConnectionStringReference value = new(postgres.Resource, false);
        string connectionString =
            await value.Resource.ConnectionStringExpression.GetValueAsync(CancellationToken.None)
            ?? throw new Exception("Unknown resource connection string");

        NpgsqlConnectionStringBuilder connectionStringBuilder = new(connectionString);

        context.EnvironmentVariables["DB_USERNAME"] =
            connectionStringBuilder.Username ?? throw new Exception("Unknown username");
        context.EnvironmentVariables["DB_PASSWORD"] =
            connectionStringBuilder.Password ?? throw new Exception("Unknown password");
    })
    .WithEnvironment(context =>
    {
        context.EnvironmentVariables["DB_URL"] =
            $"jdbc:postgresql://{postgres.Resource.PrimaryEndpoint.ContainerHost}:{postgres.Resource.PrimaryEndpoint.Port}/postgres?ApplicationName=Camunda";
    })
    .WithBindMount("./../../etc/bpmns", "/camunda/configuration/resources", true)
    .WaitFor(postgres)
    .WithHttpHealthCheck(endpointName: "dashboard", path: "/camunda/app/welcome/default/#!/login")
    .WithLifetime(ContainerLifetime.Persistent);

_ = builder
    .AddProject<Camunda_Mcp>("mcp-camunda")
    .WithEnvironment("MODE", "http")
    .WithEnvironment("CAMUNDA_HOST", camunda.GetEndpoint("dashboard"))
    .WaitFor(camunda);

await using DistributedApplication app = builder.Build();

await app.RunAsync();

return;
