using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EssentialFrame.Files;

internal sealed class DefaultFileStorage : IFileStorage
{
    private readonly IFileSystem _fileSystem;

    public DefaultFileStorage(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public string Read(string directory, string fileName, Encoding encoding = null)
    {
        string filePath = CreateFilePath(directory, fileName);

        if (!_fileSystem.File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        return _fileSystem.File.ReadAllText(filePath, encoding ?? Encoding.Unicode);
    }

    public async Task<string> ReadAsync(string directory, string fileName, Encoding encoding = null,
        CancellationToken cancellationToken = default)
    {
        string filePath = CreateFilePath(directory, fileName);

        if (!_fileSystem.File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        return await _fileSystem.File.ReadAllTextAsync(filePath, encoding ?? Encoding.Unicode, cancellationToken);
    }

    public IFileInfo Create(string directory, string fileName, string contents, Encoding encoding = null)
    {
        string filePath = CreateFilePath(directory, fileName);

        _fileSystem.File.WriteAllText(filePath, contents, encoding ?? Encoding.Unicode);
        return _fileSystem.FileInfo.New(filePath);
    }

    public async Task<IFileInfo> CreateAsync(string directory, string fileName, string contents,
        Encoding encoding = null, CancellationToken cancellationToken = default)
    {
        string filePath = CreateFilePath(directory, fileName);

        await _fileSystem.File.WriteAllTextAsync(filePath, contents, encoding ?? Encoding.Unicode, cancellationToken);
        return _fileSystem.FileInfo.New(filePath);
    }

    public void Delete(string filePath)
    {
        if (!_fileSystem.File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        _fileSystem.File.Delete(filePath);
    }

    public async Task DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        Delete(filePath);

        await Task.CompletedTask;
    }

    private string CreateFilePath(string directory, string fileName)
    {
        if (!_fileSystem.Directory.Exists(directory))
        {
            _fileSystem.Directory.CreateDirectory(directory!);
        }

        return _fileSystem.Path.Combine(directory, fileName);
    }
}