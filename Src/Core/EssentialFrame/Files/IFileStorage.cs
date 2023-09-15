using System.IO.Abstractions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EssentialFrame.Files;

public interface IFileStorage
{
    string Read(string directory, string fileName, Encoding encoding = null);

    Task<string> ReadAsync(string directory, string fileName, Encoding encoding = null,
        CancellationToken cancellationToken = default);

    IFileInfo Create(string directory, string fileName, string content, Encoding encoding = null);

    Task<IFileInfo> CreateAsync(string directory, string fileName, string content, Encoding encoding = null,
        CancellationToken cancellationToken = default);

    void Delete(string filePath);

    Task DeleteAsync(string filePath, CancellationToken cancellationToken = default);
}