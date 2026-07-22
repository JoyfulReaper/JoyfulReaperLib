using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JoyfulReaperLib.JRSerialization;

public static class JsonByteArraySerializer
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = GetJsonSerializerOptions();

    /// <summary>
    /// Serialize an object into a UTF-8 byte array.
    /// </summary>
    public static byte[]? SerializeToUtf8Bytes<T>(T value)
    {
        if (value is null)
        {
            return default;
        }

        return JsonSerializer.SerializeToUtf8Bytes(value, _jsonSerializerOptions);
    }

    /// <summary>
    /// Deserialize a UTF-8 byte array into an object of type <typeparamref name="T"/>.
    /// </summary>
    public static T? DeserializeFromUtf8Bytes<T>(byte[]? byteArray)
    {
        if (byteArray == null || byteArray.Length == 0)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(byteArray, _jsonSerializerOptions);
    }

    [Obsolete("Use SerializeToUtf8Bytes instead.")]
    public static byte[]? ObjectToByteArray(object? objData)
    {
        return SerializeToUtf8Bytes(objData);
    }

    [Obsolete("Use DeserializeFromUtf8Bytes instead.")]
    public static T? ByteArrayToObject<T>(byte[]? byteArray)
    {
        return DeserializeFromUtf8Bytes<T>(byteArray);
    }

    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = false,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }
}
