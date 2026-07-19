using System.Text.Json;
using System.Text.Json.Serialization;

namespace JoyfulReaperLib.MissionControl;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(PublishEventRequest))]
internal sealed partial class MissionControlJsonContext
    : JsonSerializerContext;