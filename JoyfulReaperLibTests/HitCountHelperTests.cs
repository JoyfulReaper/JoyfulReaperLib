/*
 * JoyfulReaperLibrary
 * 
 *  Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

using JoyfulReaperLib.JRData.Web;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLitePCL;
using System.Threading.Tasks;

namespace JoyfulReaperLibTests;

[TestClass]
public class HitCountHelperTests
{
    private SqliteConnection _connection;

    [TestInitialize]
    public void Setup()
    {
        Batteries.Init();
        _connection = new SqliteConnection("Data Source=:memory:");
    }

    [TestCleanup]
    public void Cleanup()
    {
        _connection?.Dispose();
    }

    [TestMethod]
    public async Task GetHitCounts_EmptyDatabase_ReturnsZero()
    {
        var (totalHits, uniqueVisitors) = await HitCountHelper.GetHitCounts(_connection!);

        Assert.AreEqual(0, totalHits);
        Assert.AreEqual(0, uniqueVisitors);
    }

    [TestMethod]
    public async Task ProcessHitCounts_NewIP_IncrementsCounts()
    {
        var (totalHits, uniqueVisitors) = await HitCountHelper.ProcessHitCounts(_connection!, "192.168.1.1");

        Assert.AreEqual(1, totalHits);
        Assert.AreEqual(1, uniqueVisitors);
    }

    [TestMethod]
    public async Task ProcessHitCounts_SameIP_IncrementsHitsOnly()
    {
        await HitCountHelper.ProcessHitCounts(_connection!, "192.168.1.1");
        var (totalHits, uniqueVisitors) = await HitCountHelper.ProcessHitCounts(_connection!, "192.168.1.1");

        Assert.AreEqual(2, totalHits);
        Assert.AreEqual(1, uniqueVisitors);
    }

    [TestMethod]
    public async Task ProcessHitCounts_MultipleIPs_TracksCorrectly()
    {
        await HitCountHelper.ProcessHitCounts(_connection!, "192.168.1.1");
        await HitCountHelper.ProcessHitCounts(_connection!, "192.168.1.2");
        await HitCountHelper.ProcessHitCounts(_connection!, "192.168.1.1");

        var (totalHits, uniqueVisitors) = await HitCountHelper.GetHitCounts(_connection!);

        Assert.AreEqual(3, totalHits);
        Assert.AreEqual(2, uniqueVisitors);
    }

    [TestMethod]
    public async Task ProcessHitCounts_NullIP_DoesNotIncrement()
    {
        var (totalHits, uniqueVisitors) = await HitCountHelper.ProcessHitCounts(_connection!, null!);

        Assert.AreEqual(0, totalHits);
        Assert.AreEqual(0, uniqueVisitors);
    }

    [TestMethod]
    public async Task ProcessHitCounts_EmptyIP_DoesNotIncrement()
    {
        var (totalHits, uniqueVisitors) = await HitCountHelper.ProcessHitCounts(_connection!, "");

        Assert.AreEqual(0, totalHits);
        Assert.AreEqual(0, uniqueVisitors);
    }

    [TestMethod]
    public async Task ProcessHitCounts_WhitespaceIP_DoesNotIncrement()
    {
        var (totalHits, uniqueVisitors) = await HitCountHelper.ProcessHitCounts(_connection!, "   ");

        Assert.AreEqual(0, totalHits);
        Assert.AreEqual(0, uniqueVisitors);
    }

    [TestMethod]
    public async Task ProcessHitCounts_IPv4MappedToIPv6_NormalizesToIPv4()
    {
        var ipv6Mapped = "::ffff:192.168.1.1";
        var (totalHits, uniqueVisitors) = await HitCountHelper.ProcessHitCounts(_connection!, ipv6Mapped);

        Assert.AreEqual(1, totalHits);
        Assert.AreEqual(1, uniqueVisitors);

        // Verify same IP as IPv4 increments hits, not unique visitors
        var (totalHits2, uniqueVisitors2) = await HitCountHelper.ProcessHitCounts(_connection!, "192.168.1.1");

        Assert.AreEqual(2, totalHits2);
        Assert.AreEqual(1, uniqueVisitors2);
    }

    [TestMethod]
    public async Task ProcessHitCounts_IPWithWhitespace_TrimsAndProcesses()
    {
        var (totalHits, uniqueVisitors) = await HitCountHelper.ProcessHitCounts(_connection!, "  192.168.1.1  ");

        Assert.AreEqual(1, totalHits);
        Assert.AreEqual(1, uniqueVisitors);
    }

    [TestMethod]
    public async Task ProcessHitCounts_NonIPString_StoresAsString()
    {
        var (totalHits, uniqueVisitors) = await HitCountHelper.ProcessHitCounts(_connection!, "user-session-123");

        Assert.AreEqual(1, totalHits);
        Assert.AreEqual(1, uniqueVisitors);
    }

    [TestMethod]
    public async Task GetHitCounts_AfterProcessing_ReturnsCorrectStats()
    {
        await HitCountHelper.ProcessHitCounts(_connection!, "192.168.1.1");
        await HitCountHelper.ProcessHitCounts(_connection!, "192.168.1.2");
        await HitCountHelper.ProcessHitCounts(_connection!, "192.168.1.1");
        await HitCountHelper.ProcessHitCounts(_connection!, "192.168.1.3");
        await HitCountHelper.ProcessHitCounts(_connection!, "192.168.1.2");

        var (totalHits, uniqueVisitors) = await HitCountHelper.GetHitCounts(_connection!);

        Assert.AreEqual(5, totalHits);
        Assert.AreEqual(3, uniqueVisitors);
    }

    [TestMethod]
    public async Task ProcessHitCounts_IPv6Address_StoresAsIPv6()
    {
        var ipv6Address = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";
        var (totalHits, uniqueVisitors) = await HitCountHelper.ProcessHitCounts(_connection!, ipv6Address);

        Assert.AreEqual(1, totalHits);
        Assert.AreEqual(1, uniqueVisitors);
    }

    [TestMethod]
    public async Task ProcessHitCounts_ClosedConnection_OpensAndProcesses()
    {
        _connection!.Close();
        var (totalHits, uniqueVisitors) = await HitCountHelper.ProcessHitCounts(_connection, "192.168.1.1");

        Assert.AreEqual(1, totalHits);
        Assert.AreEqual(1, uniqueVisitors);
    }
}
