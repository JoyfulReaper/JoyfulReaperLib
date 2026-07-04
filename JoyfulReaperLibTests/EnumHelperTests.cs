using JoyfulReaperLib.JREnums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoyfulReaperLibTests;

[TestClass]
public class EnumHelperTests
{
    private enum SampleEnum
    {
        One = 1,
        Two = 2
    }

    private enum ByteBackedEnum : byte
    {
        Low = 1,
        High = 250
    }

    [TestMethod]
    public void RandomEnumValue_ReturnsDefinedValue()
    {
        SampleEnum value = EnumHelper.RandomEnumValue<SampleEnum>();
        Assert.IsTrue(Enum.IsDefined(value));
    }

    [TestMethod]
    public void EnumValueIsValid_ReturnsExpectedResults()
    {
        Assert.IsTrue(EnumHelper.EnumValueIsValid(SampleEnum.One));
        Assert.IsFalse(EnumHelper.EnumValueIsValid((SampleEnum)99));
    }

    [TestMethod]
    public void GetEnumMinMax_SupportsNonIntBackingType()
    {
        (long min, long max) = EnumHelper.GetEnumMinMax<ByteBackedEnum>();

        Assert.AreEqual(1L, min);
        Assert.AreEqual(250L, max);
    }
}
