using EasySave.Services;

namespace EasySave.Tests.Services;

public class CryptoServiceTests
{
    private readonly string _testFilePath = "./testfile.txt";
    private readonly string _encryptedFilePath = "./testfile.txt.aes";
    private readonly string _decryptedFilePath = "./testfile_decrypted.txt";

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
        CryptoService.EncryptFile(_testFilePath, _encryptedFilePath);
        
        // Assert
        Assert.True(File.Exists(_encryptedFilePath));
        Assert.NotEqual("This is a test file.", File.ReadAllText(_encryptedFilePath));
    }
    
    [Fact]
    public void EncryptFile_ShouldModifyFileSize()
    {
        // Arrange
        var originalContent = "Some sample text for encryption.";
        File.WriteAllText(_testFilePath, originalContent);
        long originalSize = new FileInfo(_testFilePath).Length;
        
        // Act
        CryptoService.EncryptFile(_testFilePath, _encryptedFilePath);
        
        // Assert
        Assert.True(File.Exists(_encryptedFilePath));
        long encryptedSize = new FileInfo(_encryptedFilePath).Length;
        Assert.NotEqual(originalSize, encryptedSize);
    }

    
    [Fact]
    public void EncryptFile_ShouldBeAbleToDecrypt()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "This is a test file.");
        
        // Act
        CryptoService.EncryptFile(_testFilePath, _encryptedFilePath);
        
        // Assert
        Assert.True(File.Exists(_encryptedFilePath));
        Assert.NotEqual("This is a test file.", File.ReadAllText(_encryptedFilePath));
        
        CryptoService.DecryptFile(_encryptedFilePath, _decryptedFilePath);
        
        // Assert
        Assert.True(File.Exists(_decryptedFilePath));
        Assert.Equal("This is a test file.", File.ReadAllText(_decryptedFilePath));
    }
    
    private void Cleanup()
    {
        if (File.Exists(_testFilePath)) File.Delete(_testFilePath);
        if (File.Exists(_encryptedFilePath)) File.Delete(_encryptedFilePath);
    }
}