using JoyfulReaperLib.JRNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoyfulReaperLibTests;

[TestClass]
public class UrlValidatorTests
{
    [TestMethod]
    public void ValidateUrl_ReturnsFalseForNull()
    {
        Assert.IsFalse(UrlValidator.ValidateUrl(null));
    }

    [TestMethod]
    public void ValidateUrl_ReturnsFalseForEmpty()
    {
        Assert.IsFalse(UrlValidator.ValidateUrl(string.Empty));
    }

    [TestMethod]
    public void ValidateUrl_ReturnsFalseForWhitespace()
    {
        Assert.IsFalse(UrlValidator.ValidateUrl("   "));
    }

    [TestMethod]
    public void ValidateUrl_ReturnsFalseForRelativeUrl()
    {
        Assert.IsFalse(UrlValidator.ValidateUrl("/relative/path"));
    }

    [TestMethod]
    public void ValidateUrl_ReturnsFalseForInvalidUrl()
    {
        Assert.IsFalse(UrlValidator.ValidateUrl("not a url"));
    }

    [TestMethod]
    public void ValidateUrl_ReturnsTrueForHttpUrl()
    {
        Assert.IsTrue(UrlValidator.ValidateUrl("http://example.com"));
    }

    [TestMethod]
    public void ValidateUrl_ReturnsTrueForHttpsUrl()
    {
        Assert.IsTrue(UrlValidator.ValidateUrl("https://example.com"));
    }

    [TestMethod]
    public void ValidateUrl_ReturnsFalseForOtherSchemes()
    {
        Assert.IsFalse(UrlValidator.ValidateUrl("ftp://example.com"));
    }
}
