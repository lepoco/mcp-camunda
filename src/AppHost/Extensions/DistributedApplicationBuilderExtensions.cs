// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server Contributors.
// All Rights Reserved.

using AppHost.Models;

namespace AppHost.Extensions;

internal static class DistributedApplicationBuilderExtensions
{
    public static Credentials GetCredentials(this IDistributedApplicationBuilder builder)
    {
        return new Credentials
        {
            Postgres = new Credentials.ResourceCredentials
            {
                Username = builder.AddParameter("npgsql-user", secret: true),
                Password = builder.AddParameter("npgsql-password", secret: true),
            },
        };
    }

    public static EndpointReference GetEndpoint(
        this IDistributedApplicationBuilder builder,
        string resourceName,
        string endpointName = "http"
    )
    {
        return builder.TryGetEndpoint(resourceName, endpointName)
            ?? throw new Exception($"Resource {resourceName} endpoint not found");
    }

    public static EndpointReference? TryGetEndpoint(
        this IDistributedApplicationBuilder builder,
        string resourceName,
        string endpointName = "http"
    )
    {
        foreach (IResource resource in builder.Resources)
        {
            if (resource.Name.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase))
            {
                if (resource is IResourceWithEndpoints resourceWithEndpoints)
                {
                    return resourceWithEndpoints.GetEndpoint(endpointName);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Adds a hook to set the ASPNETCORE_FORWARDEDHEADERS_ENABLED environment variable to true for all projects in the application.
    /// </summary>
    public static IDistributedApplicationBuilder AddForwardedHeaders(
        this IDistributedApplicationBuilder builder
    )
    {
        builder.Services.TryAddLifecycleHook<AddForwardHeadersHook>();

        return builder;
    }

    private class AddForwardHeadersHook : IDistributedApplicationLifecycleHook
    {
        public Task BeforeStartAsync(
            DistributedApplicationModel appModel,
            CancellationToken cancellationToken = default
        )
        {
            foreach (var p in appModel.GetProjectResources())
            {
                p.Annotations.Add(
                    new EnvironmentCallbackAnnotation(context =>
                    {
                        context.EnvironmentVariables["ASPNETCORE_FORWARDEDHEADERS_ENABLED"] = "true";
                    })
                );
            }

            return Task.CompletedTask;
        }
    }
}
