using EasySave.Services;

namespace EasySave.Tests.Services;

public sealed class KeyManagerTests : IDisposable
{
    private const string KeyPath = ".easysave/security/encryption_key";
    private const string IvPath = ".easysave/security/encryption_iv";
    private bool _disposed;

    public KeyManagerTests()
    {
        Cleanup();
    }

    [Fact]
    public void GetKey_ShouldReturnSameKeyBetweenCalls()
    {
        // Arrange
        var keyManager = KeyManager.GetInstance();

        // Act
        byte[] key1 = keyManager.GetKey();
        byte[] key2 = keyManager.GetKey();

        // Assert
        Assert.Equal(key1, key2);
    }

    [Fact]
    public void GetIv_ShouldReturnSameIvBetweenCalls()
    {
        // Arrange
        var keyManager = KeyManager.GetInstance();

        // Act
        byte[] iv1 = keyManager.GetIv();
        byte[] iv2 = keyManager.GetIv();

        // Assert
        Assert.Equal(iv1, iv2);
    }

    [Fact]
    public void GetInstance_ShouldReturnSameInstance()
    {
        // Act
        var instance1 = KeyManager.GetInstance();
        var instance2 = KeyManager.GetInstance();

        // Assert
        Assert.Same(instance1, instance2);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Cleanup();
            }
            _disposed = true;
        }
    }

    ~KeyManagerTests()
    {
        Dispose(false);
    }

    private static void Cleanup()
    {
        if (File.Exists(KeyPath)) File.Delete(KeyPath);
        if (File.Exists(IvPath)) File.Delete(IvPath);
    }
}