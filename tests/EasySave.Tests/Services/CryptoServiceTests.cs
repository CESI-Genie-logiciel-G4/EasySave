using EasySave.Services;

namespace EasySave.Tests.Services;

public class CryptoServiceTests
{
    private readonly string _testFilePath = "testfile.txt";
    private readonly string _encryptedFilePath = "testfile.txt.aes";

    public CryptoServiceTests()
    {
        // Cleanup before tests
        Cleanup();
    }

    [Fact]
    public void EncryptFile_ShouldCreateEncryptedFile()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "This is a test file.");
        
        // Act
        CryptoService.EncryptFile(_testFilePath);
        
        // Assert
        Assert.False(File.Exists(_testFilePath)); // Original file should be deleted
        Assert.True(File.Exists(_encryptedFilePath)); // Encrypted file should exist
        Assert.NotEmpty(File.ReadAllBytes(_encryptedFilePath));
    }
    
    [Fact]
    public void EncryptFile_ShouldNotThrowException()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "Another test content.");
        
        // Act & Assert
        var exception = Record.Exception(() => CryptoService.EncryptFile(_testFilePath));
        Assert.Null(exception);
    }
    
    [Fact]
    public void EncryptFile_ShouldModifyFileSize()
    {
        // Arrange
        var originalContent = "Some sample text for encryption.";
        File.WriteAllText(_testFilePath, originalContent);
        long originalSize = new FileInfo(_testFilePath).Length;
        
        // Act
        CryptoService.EncryptFile(_testFilePath);
        
        // Assert
        Assert.True(File.Exists(_encryptedFilePath));
        long encryptedSize = new FileInfo(_encryptedFilePath).Length;
        Assert.NotEqual(originalSize, encryptedSize);
    }

    private void Cleanup()
    {
        if (File.Exists(_testFilePath)) File.Delete(_testFilePath);
        if (File.Exists(_encryptedFilePath)) File.Delete(_encryptedFilePath);
    }
}