using EasySave.Helpers;
using EasySave.Services;
using Xunit;

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
        long encryptedSize = FileHelper.GetFileSize(_encryptedFilePath);
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

    [Fact]
    public void AreFilesIdentical_ShouldReturnTrueForIdenticalFiles()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "This is a test file.");
        File.WriteAllText(_decryptedFilePath, "This is a test file.");
        
        // Act
        var result = CryptoService.AreFilesIdentical(_testFilePath, _decryptedFilePath, false);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AreFilesIdentical_ShouldReturnFalseForDifferentFiles()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "This is a test file.");
        File.WriteAllText(_decryptedFilePath, "This is a different file.");
        
        // Act
        var result = CryptoService.AreFilesIdentical(_testFilePath, _decryptedFilePath, false);
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AreFilesIdentical_ShouldReturnTrueForIdenticalEncryptedFiles()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "This is a test file.");
        CryptoService.EncryptFile(_testFilePath, _encryptedFilePath);
        
        // Act
        var result = CryptoService.AreFilesIdentical(_testFilePath, _encryptedFilePath.Replace(".aes", ""), true);
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AreFilesIdentical_ShouldReturnFalseForDifferentEncryptedFiles()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "This is a test file.");
        CryptoService.EncryptFile(_testFilePath, _encryptedFilePath);
        File.WriteAllText(_decryptedFilePath, "This is a different file.");
        CryptoService.EncryptFile(_decryptedFilePath, _decryptedFilePath + ".aes");
        
        // Act
        var result = CryptoService.AreFilesIdentical(_testFilePath, _decryptedFilePath, true);
        
        // Assert
        Assert.False(result);
    }

    private void Cleanup()
    {
        if (File.Exists(_testFilePath)) File.Delete(_testFilePath);
        if (File.Exists(_encryptedFilePath)) File.Delete(_encryptedFilePath);
        if (File.Exists(_decryptedFilePath)) File.Delete(_decryptedFilePath);
        if (File.Exists(_decryptedFilePath + ".aes")) File.Delete(_decryptedFilePath + ".aes");
        if (File.Exists(_testFilePath + ".aes")) File.Delete(_testFilePath + ".aes");
        if (File.Exists(_encryptedFilePath + ".aes")) File.Delete(_encryptedFilePath + ".aes");
    }
}