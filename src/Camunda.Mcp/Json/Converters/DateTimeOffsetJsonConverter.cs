// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and Camunda MCP Server Contributors.
// All Rights Reserved.

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Camunda.Mcp.Json.Converters;

public class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset?>
{
    private const string CamundaDateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.SSSXXX";

    public override DateTimeOffset? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            string? dateTimeString = reader.GetString();

            // Normalize timezone offset if necessary
            if (
                dateTimeString != null
                && dateTimeString.Length > 5
                && (dateTimeString[^5] == '+' || dateTimeString[^5] == '-')
                && dateTimeString[^3] != ':'
            )
            {
                dateTimeString = dateTimeString.Insert(dateTimeString.Length - 2, ":");
            }

            if (
                DateTimeOffset.TryParseExact(
                    dateTimeString,
                    CamundaDateTimeFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTimeOffset dateTimeOffset
                )
            )
            {
                return dateTimeOffset;
            }

            // Fallback: Try general deserialization
            if (
                DateTimeOffset.TryParse(
                    dateTimeString,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out dateTimeOffset
                )
            )
            {
                return dateTimeOffset;
            }

            throw new JsonException($"Invalid date format. Expected format: {CamundaDateTimeFormat}");
        }

        throw new JsonException("Unexpected token type when parsing DateTimeOffset.");
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(
                value.Value.ToString(CamundaDateTimeFormat, CultureInfo.InvariantCulture)
            );
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
