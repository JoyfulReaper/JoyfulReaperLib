using JoyfulReaperLib.JRNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoyfulReaperLibTests;

[TestClass]
public class IngressNetworkClassifierTests
{
    [TestMethod]
    [DataRow("public", IngressNetworkClassifier.Public)]
    [DataRow("dn42", IngressNetworkClassifier.Dn42)]
    [DataRow("i2p", IngressNetworkClassifier.I2p)]
    [DataRow("yggdrasil", IngressNetworkClassifier.Yggdrasil)]
    [DataRow("unknown", IngressNetworkClassifier.Unknown)]
    public void ClassificationValues_PreserveTelemetryContract(string expected, string actual)
    {
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" \t\r\n ")]
    [DataRow(".")]
    [DataRow("example.com")]
    [DataRow("randomsteam.kgivler.com")]
    [DataRow("api.ygg.kgivler.com")]
    [DataRow("dn42")]
    [DataRow("dn42.")]
    [DataRow(".dn42")]
    [DataRow("notdn42")]
    [DataRow("host.notdn42")]
    [DataRow("host.dn42.example.com")]
    [DataRow("host.dn420")]
    public void FromHost_WithoutMatchingRules_ReturnsUnknown(string? host)
    {
        Assert.AreEqual(IngressNetworkClassifier.Unknown, IngressNetworkClassifier.FromHost(host));
    }

    [TestMethod]
    [DataRow("example.dn42")]
    [DataRow("api.example.dn42")]
    [DataRow("EXAMPLE.DN42")]
    [DataRow("example.dn42.")]
    [DataRow(" \tApi.Example.DN42.\r\n ")]
    public void FromHost_Dn42Hostname_ReturnsDn42(string host)
    {
        Assert.AreEqual(IngressNetworkClassifier.Dn42, IngressNetworkClassifier.FromHost(host));
    }

    [TestMethod]
    [DataRow("example.i2p")]
    [DataRow("foo.bar.i2p")]
    [DataRow("EXAMPLE.I2P")]
    [DataRow("example.i2p.")]
    [DataRow(" \tExample.I2P.\r\n ")]
    [DataRow("example.dn42.i2p")]
    public void FromHost_I2pHostname_ReturnsI2pWithoutConfiguration(string host)
    {
        Assert.AreEqual(IngressNetworkClassifier.I2p, IngressNetworkClassifier.FromHost(host));
    }

    [TestMethod]
    [DataRow("noti2p")]
    [DataRow("examplei2p")]
    [DataRow("host.examplei2p")]
    [DataRow("i2p.example.com")]
    [DataRow("example.i2p.example.com")]
    [DataRow("example.i2p0")]
    [DataRow("i2p")]
    [DataRow("i2p.")]
    [DataRow(".i2p")]
    public void FromHost_I2pBoundaryFalsePositive_ReturnsUnknown(string host)
    {
        Assert.AreEqual(IngressNetworkClassifier.Unknown, IngressNetworkClassifier.FromHost(host));
    }

    [TestMethod]
    public void FromHost_I2pLabelBelowDn42_ReturnsDn42()
    {
        Assert.AreEqual(IngressNetworkClassifier.Dn42,
            IngressNetworkClassifier.FromHost("example.i2p.dn42"));
    }

    [TestMethod]
    [DataRow("example.i2p")]
    [DataRow("api.example.i2p")]
    [DataRow(" API.EXAMPLE.I2P. ")]
    public void FromHost_I2p_TakesPrecedenceOverConfiguredRules(string host)
    {
        Assert.AreEqual(IngressNetworkClassifier.I2p, IngressNetworkClassifier.FromHost(
            host, publicHostSuffixes: ["i2p"]));
        Assert.AreEqual(IngressNetworkClassifier.I2p, IngressNetworkClassifier.FromHost(
            host, publicHosts: [host]));
        Assert.AreEqual(IngressNetworkClassifier.I2p, IngressNetworkClassifier.FromHost(
            host,
            publicHosts: [host],
            publicHostSuffixes: ["i2p"],
            yggdrasilHostSuffixes: ["example.i2p"]));
    }

    [TestMethod]
    [DataRow("randomsteam.kgivler.com", "public")]
    [DataRow(" RANDOMSTEAM.KGIVLER.COM. ", "public")]
    [DataRow("api.randomsteam.kgivler.com", "unknown")]
    [DataRow("notrandomsteam.kgivler.com", "unknown")]
    [DataRow("randomsteam.kgivler.com.example.org", "unknown")]
    [DataRow("kgivler.com", "unknown")]
    public void FromHost_PublicHostRule_MatchesOnlyExactHost(string host, string expected)
    {
        Assert.AreEqual(expected, IngressNetworkClassifier.FromHost(
            host, publicHosts: ["other.example", " RandomSteam.Kgivler.Com. "]));
    }

    [TestMethod]
    [DataRow("kgivler.com", "public")]
    [DataRow("api.kgivler.com", "public")]
    [DataRow("nested.api.kgivler.com", "public")]
    [DataRow(" API.KGIVLER.COM. ", "public")]
    [DataRow("notkgivler.com", "unknown")]
    [DataRow("api.notkgivler.com", "unknown")]
    [DataRow("kgivler.com.example.org", "unknown")]
    [DataRow("com", "unknown")]
    public void FromHost_PublicSuffix_MatchesDomainAndSubdomains(string host, string expected)
    {
        Assert.AreEqual(expected, IngressNetworkClassifier.FromHost(
            host, publicHostSuffixes: ["other.example", " Kgivler.Com. "]));
    }

    [TestMethod]
    [DataRow("ygg.kgivler.com", "yggdrasil")]
    [DataRow("api.ygg.kgivler.com", "yggdrasil")]
    [DataRow("nested.api.ygg.kgivler.com", "yggdrasil")]
    [DataRow(" API.YGG.KGIVLER.COM. ", "yggdrasil")]
    [DataRow("notygg.kgivler.com", "unknown")]
    [DataRow("api.notygg.kgivler.com", "unknown")]
    [DataRow("ygg.kgivler.com.example.org", "unknown")]
    [DataRow("kgivler.com", "unknown")]
    public void FromHost_YggdrasilSuffix_MatchesDomainAndSubdomains(string host, string expected)
    {
        Assert.AreEqual(expected, IngressNetworkClassifier.FromHost(
            host, yggdrasilHostSuffixes: ["other.example", " Ygg.Kgivler.Com. "]));
    }

    [TestMethod]
    [DataRow("kgivler.com")]
    [DataRow("api.kgivler.com")]
    public void FromHost_LeadingDotSuffix_IncludesRootAndSubdomains(string host)
    {
        Assert.AreEqual(IngressNetworkClassifier.Public, IngressNetworkClassifier.FromHost(
            host, publicHostSuffixes: [" .KGIVLER.COM. "]));
        Assert.AreEqual(IngressNetworkClassifier.Yggdrasil, IngressNetworkClassifier.FromHost(
            host, yggdrasilHostSuffixes: [" .KGIVLER.COM. "]));
    }

    [TestMethod]
    [DataRow("api.ygg.kgivler.com")]
    [DataRow("ygg.kgivler.com")]
    public void FromHost_YggdrasilRule_TakesPrecedenceOverPublicRules(string host)
    {
        Assert.AreEqual(IngressNetworkClassifier.Yggdrasil, IngressNetworkClassifier.FromHost(
            host, publicHostSuffixes: ["kgivler.com"], yggdrasilHostSuffixes: ["ygg.kgivler.com"]));
        Assert.AreEqual(IngressNetworkClassifier.Yggdrasil, IngressNetworkClassifier.FromHost(
            host, publicHosts: [host], yggdrasilHostSuffixes: ["ygg.kgivler.com"]));
    }

    [TestMethod]
    public void FromHost_Dn42_TakesPrecedenceOverConfiguredRules()
    {
        Assert.AreEqual(IngressNetworkClassifier.Dn42, IngressNetworkClassifier.FromHost(
            "api.example.dn42",
            publicHosts: ["api.example.dn42"],
            publicHostSuffixes: ["dn42"],
            yggdrasilHostSuffixes: ["example.dn42"]));
    }

    [TestMethod]
    public void FromHost_EmptyCollections_DoNotMatch()
    {
        Assert.AreEqual(IngressNetworkClassifier.Unknown,
            IngressNetworkClassifier.FromHost("example.com", [], [], []));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" \t ")]
    [DataRow(".")]
    [DataRow("...")]
    [DataRow("example.com")]
    public void FromHost_EmptyRuleEntries_DoNotMatch(string? host)
    {
        string?[] rules = [null, "", " \t ", ".", "..."];
        Assert.AreEqual(IngressNetworkClassifier.Unknown,
            IngressNetworkClassifier.FromHost(host, rules, rules, rules));
    }

    [TestMethod]
    public void FromHost_EmptyRuleEntries_DoNotHideLaterMatches()
    {
        string?[] rules = [null, "", " ", ".", "example.com"];
        Assert.AreEqual(IngressNetworkClassifier.Public,
            IngressNetworkClassifier.FromHost("example.com", publicHosts: rules));
        Assert.AreEqual(IngressNetworkClassifier.Public,
            IngressNetworkClassifier.FromHost("api.example.com", publicHostSuffixes: rules));
        Assert.AreEqual(IngressNetworkClassifier.Yggdrasil,
            IngressNetworkClassifier.FromHost("api.example.com", yggdrasilHostSuffixes: rules));
    }
}
