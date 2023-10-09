using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EssentialFrame.Files.Interfaces;

public interface IFileStorage
{
    string Read(string directory, string fileName, Encoding encoding);

    Task<string> ReadAsync(string directory, string fileName, Encoding encoding, CancellationToken cancellationToken);

    IFileInfo Create(string directory, string fileName, string content, Encoding encoding);

    Task<IFileInfo> CreateAsync(string directory, string fileName, string content, Encoding encoding,
        CancellationToken cancellationToken);

    void Delete(string filePath);

    Task DeleteAsync(string filePath, CancellationToken cancellationToken);
}