using System;
using System.IO;
using System.Threading.Tasks;

namespace FileSystemWatcherCommon
{
    public interface IFileSystemWatcherCommon
    {
        void Watch(string path, NotifyFilters notifyFilters = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size, string filter = null, bool includeSubdirectories = false);
        void OnCreated(object source, FileSystemEventArgs e);
        void OnDeleted(object source, FileSystemEventArgs e);
        void OnChanged(object source, FileSystemEventArgs e);
        void OnRenamed(object source, RenamedEventArgs e);
        void OnError(object source, ErrorEventArgs e);
        void TimeoutFileAction<T>(Func<T> func, string filePath);
        T TimeoutFileActionGet<T>(Func<T> func, string filePath);
        string SetFileName(string filePath);
        Task<string> SetFileNameAsync(string filePath);
        void MoveFileToErrorDirectory(string filePath);
        Task MoveFileToErrorDirectoryAsync(string filePath);
        void MoveFileToOutDirectory(string filePath);
        Task MoveFileToOutDirectoryAsync(string filePath);
        void ReplaceFileToErrorDirectory(string filePath);
        Task ReplaceFileToErrorDirectoryAsync(string filePath);
        void ReplaceFileToOutDirectory(string filePath);
        Task ReplaceFileToOutDirectoryAsync(string filePath);
    }
}