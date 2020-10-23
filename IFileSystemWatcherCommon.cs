using System.IO;
using System.Threading.Tasks;

namespace FileSystemWatcherCommon
{
    public interface IFileSystemWatcherCommon
    {
        void Watch(string path, NotifyFilters notifyFilters = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size, string filter = null);
        void OnCreated(object source, FileSystemEventArgs e);
        void OnDeleted(object source, FileSystemEventArgs e);
        void OnChanged(object source, FileSystemEventArgs e);
        void OnRenamed(object source, RenamedEventArgs e);
        void OnError(object source, ErrorEventArgs e);
        string SetFileName(string filePath);
        Task<string> SetFileNameAsync(string filePath);
        void MoveFileToErrorDirectory(string filePath);
        Task MoveFileToErrorDirectoryAsync(string filePath);
        void MoveFileToOutDirectory(string filePath);
        Task MoveFileToOutDirectoryAsync(string filePath);
    }
}