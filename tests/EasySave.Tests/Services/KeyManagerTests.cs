using EasySave.Services;

namespace EasySave.Tests.Services;

public class KeyManagerTests : IDisposable
    {
        private const string KeyPath = ".easysave/security/encryption_key";
        private const string IvPath = ".easysave/security/encryption_iv";

        public KeyManagerTests()
        {
            Cleanup();
        }
        
        [Fact]
        public void GetIv_ShouldReturnValidIv()
        {
            // Act
            var keyManager = KeyManager.GetInstance();
            byte[] iv = keyManager.GetIv();

            // Assert
            Assert.NotNull(iv);
            Assert.True(File.Exists(IvPath));
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

        public void Dispose()
        {
            Cleanup();
        }

        private static void Cleanup()
        {
            if (File.Exists(KeyPath)) File.Delete(KeyPath);
            if (File.Exists(IvPath)) File.Delete(IvPath);
        }
    }