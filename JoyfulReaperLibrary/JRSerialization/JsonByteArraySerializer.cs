//https://stackoverflow.com/questions/64799591/is-there-a-high-performance-way-to-replace-the-binaryformatter-in-net5

using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;

namespace JoyfulReaperLib.JRSerialization;

public class JsonByteArraySerializer
{
    /// <summary>
    /// Convert an object to a Byte Array.
    /// </summary>
    public static byte[] ObjectToByteArray(object objData)
    {
        if (objData == null)
            return default;

        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(objData, GetJsonSerializerOptions()));
    }

    /// <summary>
    /// Convert a byte array to an Object of T.
    /// </summary>
    public static T ByteArrayToObject<T>(byte[] byteArray)
    {
        if (byteArray == null || !byteArray.Any())
            return default;

        return JsonSerializer.Deserialize<T>(byteArray, GetJsonSerializerOptions());
    }

    private static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions() {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
    }
}
