using JoyfulReaperLib.JREncryption;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JoyfulReaperLibTests;

[TestClass]
public class CipherHelperTests
{
    [TestMethod]
    public void CaesarCipher_RoundTrips()
    {
        string encoded = CipherHelper.CaesarEncipher("Hello, World!", 3, stripNonAlphaChars: false);
        string decoded = CipherHelper.CaesarDecipher(encoded, 3);

        Assert.AreEqual("Hello, World!", decoded);
    }

    [TestMethod]
    public void CaesarCipher_SupportsNegativeAndLargeKeys()
    {
        Assert.AreEqual("ZAB", CipherHelper.CaesarEncipher("ABC", -1));
        Assert.AreEqual("BCD", CipherHelper.CaesarEncipher("ABC", 27));
    }

    [TestMethod]
    public void VigenereCipher_UsesKnownExample()
    {
        string output = CipherHelper.VigenereCipher("ATTACK AT DAWN!", "LEMON", CipherHelper.Direction.Encipher, stripNonAlphaChars: false);

        Assert.AreEqual("LXFOPV EF RNHR!", output);
    }

    [TestMethod]
    public void VigenereCipher_RejectsEmptyKey()
    {
        Assert.Throws<ArgumentException>(() =>
            CipherHelper.VigenereCipher("ATTACK", string.Empty, CipherHelper.Direction.Encipher));
    }

    [TestMethod]
    public void VigenereCipher_PreservesOrStripsPunctuationAsRequested()
    {
        Assert.AreEqual("B", CipherHelper.VigenereCipher("A!", "B", CipherHelper.Direction.Encipher, stripNonAlphaChars: true));
        Assert.AreEqual("B!", CipherHelper.VigenereCipher("A!", "B", CipherHelper.Direction.Encipher, stripNonAlphaChars: false));
    }
}
