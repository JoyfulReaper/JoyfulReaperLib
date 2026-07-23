using JoyfulReaperLib.JRArray;
using JoyfulReaperLib.JRLists;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoyfulReaperLibTests;

[TestClass]
public class RandomItemTests
{
    [TestMethod]
    public void ArrayRandomItem_RejectsNullAndEmpty()
    {
        Assert.Throws<ArgumentNullException>(() => ArrayExtensions.RandomItem<string>(null!));
        Assert.Throws<ArgumentException>(() => Array.Empty<int>().RandomItem());
    }

    [TestMethod]
    public void ListRandomItem_RejectsNullAndEmpty()
    {
        Assert.Throws<ArgumentNullException>(() => ListExtensions.RandomItem<string>(null!));
        Assert.Throws<ArgumentException>(() => new List<int>().RandomItem());
    }

    [TestMethod]
    public void RandomItem_ReturnsExistingValue()
    {
        int[] array = [1, 2, 3];
        List<int> list = [4, 5, 6];
        IReadOnlyList<int> readOnly = list;

        int arrayValue = array.RandomItem();
        int listValue = list.RandomItem();
        int readOnlyValue = readOnly.RandomItem();

        CollectionAssert.Contains(array, arrayValue);
        CollectionAssert.Contains(list, listValue);
        CollectionAssert.Contains(list, readOnlyValue);
    }
}
