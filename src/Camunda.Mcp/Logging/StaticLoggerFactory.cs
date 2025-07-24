// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server Contributors.
// All Rights Reserved.

namespace Camunda.Mcp.Logging;

internal static class StaticLoggerFactory
{
    public static ILogger New(string category = "Camunda.MCP") =>
        LoggerFactory
            .Create(loggerBuilder =>
            {
                loggerBuilder.AddConsole(consoleLogOptions =>
                {
                    // Configure all logs to go to stderr
                    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
                });
                loggerBuilder.SetMinimumLevel(LogLevel.Trace);
            })
            .CreateLogger(category);
}
