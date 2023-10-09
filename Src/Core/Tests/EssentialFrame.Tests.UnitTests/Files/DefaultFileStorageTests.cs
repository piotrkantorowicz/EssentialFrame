using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Files;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Tests.UnitTests.Files;

[TestFixture]
public class DefaultFileStorageTests
{
    private readonly Faker _faker = new();
    private readonly CancellationToken _cancellationToken = default;

    private Mock<IFileSystem> _fileSystemMock;
    
    [SetUp]
    public void SetUp()
    {
        _fileSystemMock = new Mock<IFileSystem>();
    }

    [TearDown]
    public void TearDown()
    {
        _fileSystemMock.Reset();
    }
    
    [Test]
    public void Read_WhenDirectoryExists_ShouldReadFile()
    {
        // Arrange
        DefaultFileStorage fileStorage = new(_fileSystemMock.Object);
        string directory = _faker.System.DirectoryPath();
        string fileName = _faker.System.FileName();
        Encoding encoding = Encoding.UTF8;
        string filePath = Path.Combine(directory, fileName);

        _fileSystemMock.Setup(x => x.Directory.Exists(directory)).Returns(true);
        _fileSystemMock.Setup(x => x.Path.Combine(directory, fileName)).Returns(filePath);
        _fileSystemMock.Setup(x => x.File.Exists(filePath)).Returns(true);
        _fileSystemMock.Setup(x => x.File.ReadAllText(filePath, encoding));

        // Act
        fileStorage.Read(directory, fileName, encoding);

        // Assert
        _fileSystemMock.Verify(x => x.File.ReadAllText(filePath, encoding), Times.Once);
    }

    [Test]
    public async Task ReadAsync_WhenFileExists_ShouldReadFile()
    {
        // Arrange
        DefaultFileStorage fileStorage = new(_fileSystemMock.Object);
        string directory = _faker.System.DirectoryPath();
        string fileName = _faker.System.FileName();
        Encoding encoding = Encoding.UTF8;
        string filePath = Path.Combine(directory, fileName);

        _fileSystemMock.Setup(x => x.Directory.Exists(directory)).Returns(true);
        _fileSystemMock.Setup(x => x.Path.Combine(directory, fileName)).Returns(filePath);
        _fileSystemMock.Setup(x => x.File.Exists(filePath)).Returns(true);
        _fileSystemMock.Setup(x => x.File.ReadAllTextAsync(filePath, encoding, _cancellationToken));

        // Act
        await fileStorage.ReadAsync(directory, fileName, encoding, _cancellationToken);

        // Assert
        _fileSystemMock.Verify(x => x.File.ReadAllTextAsync(filePath, encoding, _cancellationToken), Times.Once);
    }

    [Test]
    public void Read_WhenFileDoesNotExists_ShouldThrowFileNotFoundException()
    {
        // Arrange
        DefaultFileStorage fileStorage = new(_fileSystemMock.Object);
        string directory = _faker.System.DirectoryPath();
        string fileName = _faker.System.FileName();
        Encoding encoding = Encoding.UTF8;
        string filePath = Path.Combine(directory, fileName);

        _fileSystemMock.Setup(x => x.Directory.Exists(directory)).Returns(true);
        _fileSystemMock.Setup(x => x.Path.Combine(directory, fileName)).Returns(filePath);
        _fileSystemMock.Setup(x => x.File.Exists(filePath)).Returns(false);
        _fileSystemMock.Setup(x => x.File.ReadAllText(filePath, encoding));

        // Act
        Action action = () => fileStorage.Read(directory, fileName, encoding);

        // Assert
        action.Should().ThrowExactly<FileNotFoundException>().WithMessage($"File not found: {filePath}");
    }

    [Test]
    public async Task ReadAsync_WhenFileDoesNotExists_ShouldThrowFileNotFoundException()
    {
        // Arrange
        DefaultFileStorage fileStorage = new(_fileSystemMock.Object);
        string directory = _faker.System.DirectoryPath();
        string fileName = _faker.System.FileName();
        Encoding encoding = Encoding.UTF8;
        string filePath = Path.Combine(directory, fileName);

        _fileSystemMock.Setup(x => x.Directory.Exists(directory)).Returns(true);
        _fileSystemMock.Setup(x => x.Path.Combine(directory, fileName)).Returns(filePath);
        _fileSystemMock.Setup(x => x.File.Exists(filePath)).Returns(false);
        _fileSystemMock.Setup(x => x.File.ReadAllTextAsync(filePath, encoding, _cancellationToken));

        // Act
        Func<Task> action = async () => await fileStorage.ReadAsync(directory, fileName, encoding, _cancellationToken);

        // Assert
        await action.Should().ThrowExactlyAsync<FileNotFoundException>().WithMessage($"File not found: {filePath}");
    }

    [Test]
    public void Create_WhenDirectoryExists_ShouldOnlyCreateFile()
    {
        // Arrange
        DefaultFileStorage fileStorage = new(_fileSystemMock.Object);
        string directory = _faker.System.DirectoryPath();
        string fileName = _faker.System.FileName();
        string contents = _faker.Lorem.Sentence();
        Encoding encoding = Encoding.UTF8;
        string filePath = Path.Combine(directory, fileName);
        Mock<IFileInfo> fileInfoMock = new();

        fileInfoMock.Setup(x => x.Length).Returns(contents.Length);
        _fileSystemMock.Setup(x => x.Directory.Exists(directory)).Returns(true);
        _fileSystemMock.Setup(x => x.Path.Combine(directory, fileName)).Returns(filePath);
        _fileSystemMock.Setup(x => x.File.WriteAllText(filePath, contents, encoding));
        _fileSystemMock.Setup(x => x.FileInfo.New(filePath)).Returns(fileInfoMock.Object);

        // Act
        IFileInfo fileInfo = fileStorage.Create(directory, fileName, contents, encoding);

        // Assert
        _fileSystemMock.Verify(x => x.File.WriteAllText(filePath, contents, encoding), Times.Once);

        fileInfo.Length.Should().Be(contents.Length);
    }

    [Test]
    public void Create_WhenDirectoryDoesNotExists_ShouldCreateDirectoryAndFile()
    {
        // Arrange
        DefaultFileStorage fileStorage = new(_fileSystemMock.Object);
        string directory = _faker.System.DirectoryPath();
        string fileName = _faker.System.FileName();
        string contents = _faker.Lorem.Sentence();
        Encoding encoding = Encoding.UTF8;
        string filePath = Path.Combine(directory, fileName);
        Mock<IFileInfo> fileInfoMock = new();

        fileInfoMock.Setup(x => x.Length).Returns(contents.Length);
        _fileSystemMock.Setup(x => x.Directory.Exists(directory)).Returns(false);
        _fileSystemMock.Setup(x => x.Path.Combine(directory, fileName)).Returns(filePath);
        _fileSystemMock.Setup(x => x.Directory.CreateDirectory(directory));
        _fileSystemMock.Setup(x => x.File.WriteAllText(filePath, contents, encoding));
        _fileSystemMock.Setup(x => x.FileInfo.New(filePath)).Returns(fileInfoMock.Object);

        // Act
        IFileInfo fileInfo = fileStorage.Create(directory, fileName, contents, encoding);

        // Assert
        _fileSystemMock.Verify(x => x.Directory.Exists(directory), Times.Once);
        _fileSystemMock.Verify(x => x.Path.Combine(directory, fileName), Times.Once);
        _fileSystemMock.Verify(x => x.Directory.CreateDirectory(directory), Times.Once);
        _fileSystemMock.Verify(x => x.File.WriteAllText(filePath, contents, encoding), Times.Once);

        fileInfo.Length.Should().Be(contents.Length);
    }

    [Test]
    public async Task CreateAsync_WhenDirectoryExists_ShouldCreateFile()
    {
        // Arrange
        DefaultFileStorage fileStorage = new(_fileSystemMock.Object);
        string directory = _faker.System.DirectoryPath();
        string fileName = _faker.System.FileName();
        string contents = _faker.Lorem.Sentence();
        Encoding encoding = Encoding.UTF8;
        string filePath = Path.Combine(directory, fileName);
        Mock<IFileInfo> fileInfoMock = new();

        fileInfoMock.Setup(x => x.Length).Returns(contents.Length);
        _fileSystemMock.Setup(x => x.Directory.Exists(directory)).Returns(true);
        _fileSystemMock.Setup(x => x.Path.Combine(directory, fileName)).Returns(filePath);
        _fileSystemMock.Setup(x => x.File.WriteAllText(filePath, contents, encoding));
        _fileSystemMock.Setup(x => x.FileInfo.New(filePath)).Returns(fileInfoMock.Object);

        // Act
        IFileInfo fileInfo = await fileStorage.CreateAsync(directory, fileName, contents, encoding, _cancellationToken);

        // Assert
        _fileSystemMock.Verify(x => x.File.WriteAllTextAsync(filePath, contents, encoding, _cancellationToken),
            Times.Once);

        fileInfo.Length.Should().Be(contents.Length);
    }

    [Test]
    public async Task CreateAsync_WhenDirectoryDoesNotExists_ShouldCreateDirectoryAndFile()
    {
        // Arrange
        DefaultFileStorage fileStorage = new(_fileSystemMock.Object);
        string directory = _faker.System.DirectoryPath();
        string fileName = _faker.System.FileName();
        string contents = _faker.Lorem.Sentence();
        Encoding encoding = Encoding.UTF8;
        string filePath = Path.Combine(directory, fileName);
        Mock<IFileInfo> fileInfoMock = new();

        fileInfoMock.Setup(x => x.Length).Returns(contents.Length);
        _fileSystemMock.Setup(x => x.Directory.Exists(directory)).Returns(false);
        _fileSystemMock.Setup(x => x.Path.Combine(directory, fileName)).Returns(filePath);
        _fileSystemMock.Setup(x => x.Directory.CreateDirectory(directory));
        _fileSystemMock.Setup(x => x.File.WriteAllTextAsync(filePath, contents, encoding, _cancellationToken));
        _fileSystemMock.Setup(x => x.FileInfo.New(filePath)).Returns(fileInfoMock.Object);

        // Act
        IFileInfo fileInfo = await fileStorage.CreateAsync(directory, fileName, contents, encoding, _cancellationToken);

        // Assert
        _fileSystemMock.Verify(x => x.Directory.Exists(directory), Times.Once);
        _fileSystemMock.Verify(x => x.Path.Combine(directory, fileName), Times.Once);
        _fileSystemMock.Verify(x => x.Directory.CreateDirectory(directory), Times.Once);
        _fileSystemMock.Verify(x => x.File.WriteAllTextAsync(filePath, contents, encoding, _cancellationToken),
            Times.Once);

        fileInfo.Length.Should().Be(contents.Length);
    }

    [Test]
    public void Delete_WhenFileExists_ShouldDeleteFile()
    {
        // Arrange
        DefaultFileStorage fileStorage = new(_fileSystemMock.Object);
        string filePath = _faker.System.FilePath();

        _fileSystemMock.Setup(x => x.File.Exists(filePath)).Returns(true);
        _fileSystemMock.Setup(x => x.File.Delete(filePath));

        // Act
        fileStorage.Delete(filePath);

        // Assert
        _fileSystemMock.Verify(x => x.File.Delete(filePath), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WhenFileExists_ShouldDeleteFile()
    {
        // Arrange
        DefaultFileStorage fileStorage = new(_fileSystemMock.Object);
        string filePath = _faker.System.FilePath();

        _fileSystemMock.Setup(x => x.File.Exists(filePath)).Returns(true);
        _fileSystemMock.Setup(x => x.File.Delete(filePath));

        // Act
        await fileStorage.DeleteAsync(filePath, _cancellationToken);

        // Assert
        _fileSystemMock.Verify(x => x.File.Delete(filePath), Times.Once);
    }

    [Test]
    public void Delete_WhenFileDoesNotExists_ShouldThrowFileNotFoundException()
    {
        // Arrange
        DefaultFileStorage fileStorage = new(_fileSystemMock.Object);
        string filePath = _faker.System.FilePath();

        _fileSystemMock.Setup(x => x.File.Exists(filePath)).Returns(false);
        _fileSystemMock.Setup(x => x.File.Delete(filePath));

        // Act
        Action action = () => fileStorage.Delete(filePath);

        // Assert
        action.Should().ThrowExactly<FileNotFoundException>().WithMessage($"File not found: {filePath}");
    }

    [Test]
    public async Task DeleteAsync_WhenFileDoesNotExists_ShouldThrowFileNotFoundException()
    {
        // Arrange
        DefaultFileStorage fileStorage = new(_fileSystemMock.Object);
        string filePath = _faker.System.FilePath();

        _fileSystemMock.Setup(x => x.File.Exists(filePath)).Returns(false);
        _fileSystemMock.Setup(x => x.File.Delete(filePath));

        // Act
        Func<Task> action = async () => await fileStorage.DeleteAsync(filePath, _cancellationToken);

        // Assert
        await action.Should().ThrowExactlyAsync<FileNotFoundException>().WithMessage($"File not found: {filePath}");
    }
}