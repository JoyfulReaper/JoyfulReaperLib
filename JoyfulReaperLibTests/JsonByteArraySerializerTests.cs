using System.Text;
using JoyfulReaperLib.JRSerialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoyfulReaperLibTests;

[TestClass]
public class JsonByteArraySerializerTests
{
    private sealed record SamplePayload(string Name, int Count);

    [TestMethod]
    public void SerializeAndDeserialize_RoundTrip()
    {
        SamplePayload payload = new("hello", 2);

        byte[]? bytes = JsonByteArraySerializer.SerializeToUtf8Bytes(payload);
        SamplePayload? deserialized = JsonByteArraySerializer.DeserializeFromUtf8Bytes<SamplePayload>(bytes);

        Assert.IsNotNull(bytes);
        CollectionAssert.AreEqual(Encoding.UTF8.GetBytes("{\"Name\":\"hello\",\"Count\":2}"), bytes);
        Assert.AreEqual(payload, deserialized);
    }

    [TestMethod]
    public void Serialize_Null_ReturnsNull()
    {
        Assert.IsNull(JsonByteArraySerializer.SerializeToUtf8Bytes<string>(null!));
    }

    [TestMethod]
    public void Deserialize_NullOrEmpty_ReturnsDefault()
    {
        Assert.IsNull(JsonByteArraySerializer.DeserializeFromUtf8Bytes<SamplePayload>(null));
        Assert.IsNull(JsonByteArraySerializer.DeserializeFromUtf8Bytes<SamplePayload>([]));
    }
}
