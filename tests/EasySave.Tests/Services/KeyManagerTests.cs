using EasySave.Services;

namespace EasySave.Tests.Services;

public class KeyManagerTests
{
    private const string KeyPath = "encryption_key.txt";
    private const string IvPath = "encryption_iv.txt";

    public KeyManagerTests()
    {
        Cleanup();
    }

    [Fact]
    public void GetOrCreateKey_ShouldReturnValidKey()
    {
        // Act
        byte[] key = KeyManager.GetOrCreateKey();
        
        // Assert
        Assert.NotNull(key);
        Assert.Equal(32, key.Length); // AES-256 key length
        Assert.True(File.Exists(KeyPath));
    }

    [Fact]
    public void GetOrCreateIv_ShouldReturnValidIv()
    {
        // Act
        byte[] iv = KeyManager.GetOrCreateIv();
        
        // Assert
        Assert.NotNull(iv);
        Assert.Equal(16, iv.Length); // AES block size
        Assert.True(File.Exists(IvPath));
    }

    [Fact]
    public void GetOrCreateKey_ShouldPersistBetweenCalls()
    {
        // Act
        byte[] key1 = KeyManager.GetOrCreateKey();
        byte[] key2 = KeyManager.GetOrCreateKey();
        
        // Assert
        Assert.Equal(key1, key2);
    }

    [Fact]
    public void GetOrCreateIv_ShouldPersistBetweenCalls()
    {
        // Act
        byte[] iv1 = KeyManager.GetOrCreateIv();
        byte[] iv2 = KeyManager.GetOrCreateIv();
        
        // Assert
        Assert.Equal(iv1, iv2);
    }

    private static void Cleanup()
    {
        if (File.Exists(KeyPath)) File.Delete(KeyPath);
        if (File.Exists(IvPath)) File.Delete(IvPath);
    }
}