using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystemWatcherCommon
{
    #region public abstract class FileSystemWatcherCommon : IFileSystemWatcherCommon
    /// <summary>
    /// Abstrakcyjna, wspólna klasa obserwacji zdarzeń plików w katalogu
    /// Abstract common class for observing files in a directory
    /// </summary>
    public abstract class FileSystemWatcherCommon : IFileSystemWatcherCommon
    {
        #region public virtual event EventHandler<FileSystemEventArgs> OnCreatedEventHandler;
        /// <summary>
        /// Obsługa zdarzeń dla zdarzenia tworzenia pliku
        /// Event handling for the create file event
        /// </summary>
        public virtual event EventHandler<FileSystemEventArgs> OnCreatedEventHandler;
        #endregion

        #region public virtual event EventHandler<FileSystemEventArgs> OnChangedEventHandler;
        /// <summary>
        /// Obsługa zdarzeń dla zdarzenia zmiany parametrów pliku
        /// Event handling for the file parameter change event
        /// </summary>
        public virtual event EventHandler<FileSystemEventArgs> OnChangedEventHandler;
        #endregion

        #region public virtual event EventHandler<FileSystemEventArgs> OnRenamedEventHandler;
        /// <summary>
        /// Obsługa zdarzeń dla zdarzenia zmiany nazwy pliku
        /// Event handling for the file rename event
        /// </summary>
        public virtual event EventHandler<FileSystemEventArgs> OnRenamedEventHandler;
        #endregion

        #region public virtual event EventHandler<FileSystemEventArgs> OnDeletedEventHandler;
        /// <summary>
        /// Obsługa zdarzeń dla zdarzenia usunięcia pliku
        /// Event handler for the file delete event
        /// </summary>
        public virtual event EventHandler<FileSystemEventArgs> OnDeletedEventHandler;
        #endregion

        #region public virtual event EventHandler<ErrorEventArgs> OnErrorEventHandler;
        /// <summary>
        /// Obsługa zdarzeń dla zdarzenia błędu
        /// Event handling for the error event
        /// </summary>
        public virtual event EventHandler<ErrorEventArgs> OnErrorEventHandler;
        #endregion

        #region public virtual void Watch...
        /// <summary>
        /// Obserwuj katalog z plikami i wykonaj odpowiednią akcję zdarzenia
        /// Watch the directory with files and perform the appropriate event action
        /// </summary>
        /// <param name="path">
        /// Śćieżka do katalogu
        /// Path to directory
        /// </param>
        /// <param name="notifyFilters">
        /// Filtry powiadomienia wyzwalające notyfikację
        /// Notification filters that trigger a notification
        /// </param>
        /// <param name="filter">
        /// Filtry rozszerzeń plików
        /// Filters of file extensions
        /// </param>
        public virtual void Watch(string path, NotifyFilters notifyFilters = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size, string filter = null)
        {
            using (FileSystemWatcher fileSystemWatcher = new FileSystemWatcher())
            {
                if (Directory.Exists(path))
                {
                    fileSystemWatcher.Path = path;
                    fileSystemWatcher.InternalBufferSize = 64 * 1024;
                    fileSystemWatcher.NotifyFilter = notifyFilters;
                    if (null != filter && !string.IsNullOrWhiteSpace(filter))
                    {
                        List<string> filterList = NetAppCommon.Helpers.Lists.ListsHelper.GetInstance().ConvertToListOfString(filter, new char[] { ',', ';', '|' });
                        if (!(null != filterList && filterList.Count > 0))
                        {
                            fileSystemWatcher.Filter = filter;
                        }
                    }
                    fileSystemWatcher.Created += OnCreated;
                    fileSystemWatcher.Changed += OnChanged;
                    fileSystemWatcher.Renamed += OnRenamed;
                    fileSystemWatcher.Deleted += OnDeleted;
                    fileSystemWatcher.Error += OnError;
                    fileSystemWatcher.EnableRaisingEvents = true;
                    while (true)
                    {
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    throw new Exception($"Directory { path } do not exists!");
                }
            }
        }
        #endregion

        #region public virtual void OnChanged(object source, FileSystemEventArgs e)
        /// <summary>
        /// Uruchom zdarzenie OnChange w obserwowanych katalogu
        /// Run the OnChange event in the watch directory
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public virtual void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed.
            OnChangedEventHandler?.Invoke(source, e);
        }
        #endregion

        #region public virtual void OnCreated(object source, FileSystemEventArgs e)
        /// <summary>
        /// Uruchom zdarzenie OnCreated w obserwowanych katalogu
        /// Run the OnCreated event in the watch directory
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public virtual void OnCreated(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is created.
            OnCreatedEventHandler?.Invoke(source, e);
        }
        #endregion

        #region public virtual void OnRenamed(object source, RenamedEventArgs e)
        /// <summary>
        /// Uruchom zdarzenie OnRenamed w obserwowanych katalogu
        /// Run the OnRenamed event in the watch directory
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public virtual void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            OnRenamedEventHandler?.Invoke(source, e);
        }
        #endregion

        #region public virtual void OnDeleted(object source, FileSystemEventArgs e)
        /// <summary>
        /// Uruchom zdarzenie OnDeleted w obserwowanych katalogu
        /// Run the OnDeleted event in the watch directory
        /// </summary>
        public virtual void OnDeleted(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed.
            OnDeletedEventHandler?.Invoke(source, e);
        }
        #endregion

        #region public virtual void OnError(object source, ErrorEventArgs e)
        /// <summary>
        /// Uruchom zdarzenie OnError w obserwowanych katalogu
        /// Run the OnError event in the watch directory
        /// </summary>
        public virtual void OnError(object source, ErrorEventArgs e)
        {
            // Specify what is error.
            OnErrorEventHandler?.Invoke(source, e);
        }
        #endregion

        public virtual void TimeoutFileAction<T>(Func<T> func, string filePath)
        {
            NetAppCommon.Helpers.Files.FileHelper.GetInstance().TimeoutAction(func, filePath);
        }

        public T TimeoutFileActionGet<T>(Func<T> func, string filePath)
        {
            return NetAppCommon.Helpers.Files.FileHelper.GetInstance().TimeoutActionReturn(func, filePath);
        }

        #region public virtual string SetFileName(string filePath)
        /// <summary>
        /// Ustaw nową nazwę pliku do przeniesienia do innego katalogu
        /// Set a new file name to be moved to another directory
        /// </summary>
        /// <param name="filePath">
        /// Ścieżka do pliku jako string
        /// File path as string
        /// </param>
        /// <returns>
        /// Nowa nazwa pliku jako string lub null
        /// New filename as string or null
        /// </returns>
        public virtual string SetFileName(string filePath)
        {
            try
            {
                if (null != filePath && !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    string fileName = Path.GetFileName(filePath);
                    if (null != fileName && !string.IsNullOrWhiteSpace(fileName))
                    {
                        string pattern = @"\[(.*?)\]\.?";
                        MatchCollection matchCollection = Regex.Matches(fileName, pattern);
                        if (null != matchCollection && matchCollection.Count >= 2 && null != matchCollection[0].Value && !string.IsNullOrWhiteSpace(matchCollection[0].Value))
                        {
                            fileName = fileName.Replace(matchCollection[0].Value, string.Empty);
                        }
                        fileName = Regex.Replace(Regex.Replace(fileName, @"\.+", "."), @"\[+|\]+", string.Empty).Replace(Path.GetExtension(filePath), string.Empty);
                        return string.Format("[{0}.{1}.{2}.{3}][{4}]{5}", DateTime.Now.ToString("HH"), DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond, fileName, Path.GetExtension(filePath));
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return null;
        }
        #endregion

        #region public virtual async Task<string> SetFileNameAsync(string filePath)
        /// <summary>
        /// Ustaw nową nazwę pliku do przeniesienia do innego katalogu asynchronicznie
        /// Set a new file name to be moved to another directory asynchronously
        /// </summary>
        /// <param name="filePath">
        /// Ścieżka do pliku jako string
        /// File path as string
        /// </param>
        /// <returns>
        /// Nowa nazwa pliku jako string lub null
        /// New filename as string or null
        /// </returns>
        public virtual async Task<string> SetFileNameAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                return SetFileName(filePath);
            });
        }
        #endregion

        #region public virtual void MoveFileToErrorDirectory(string filePath)
        /// <summary>
        /// Przenieś plik do katalogu z plikami po zarejestrowaniu błędu w migracji
        /// Move the file to the files directory after logging the migration error
        /// </summary>
        /// <param name="filePath">
        /// Ścieżka do pliku jako string
        /// File path as string
        /// </param>
        public virtual void MoveFileToErrorDirectory(string filePath)
        {
            try
            {
                if (null != filePath && !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    string destFileName = SetFileName(filePath);
                    string destFilePath = Path.Combine(Path.GetDirectoryName(filePath), string.Format("{0}{1}", @"..", Path.DirectorySeparatorChar.ToString()), "error", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), destFileName);
                    if (!Directory.Exists(Path.GetDirectoryName(destFilePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destFilePath));
                    }
                    if (!File.Exists(destFilePath))
                    {
                        File.Move(filePath, destFilePath);
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region public virtual async Task MoveFileToErrorDirectoryAsync(string filePath)
        /// <summary>
        /// Przenieś plik do katalogu z plikami po zarejestrowaniu błędu w migracji asynchronicznie
        /// Move the file to the files directory after logging the migration error asynchronously
        /// </summary>
        /// <param name="filePath">
        /// Ścieżka do pliku jako string
        /// File path as string
        /// </param>
        public virtual async Task MoveFileToErrorDirectoryAsync(string filePath)
        {
            await Task.Run(() =>
            {
                MoveFileToErrorDirectory(filePath);
            });
        }
        #endregion

        #region public virtual void MoveFileToOutDirectory(string filePath)
        /// <summary>
        /// Przenieś plik do katalogu z plikami po poprawnym wykonaniu operacji
        /// Move the file to the directory with files after the correct operation
        /// </summary>
        /// <param name="filePath">
        /// Ścieżka do pliku jako string
        /// File path as string
        /// </param>
        public virtual void MoveFileToOutDirectory(string filePath)
        {
            try
            {
                if (null != filePath && !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    string destFileName = SetFileName(filePath);
                    string destFilePath = Path.Combine(Path.GetDirectoryName(filePath), string.Format("{0}{1}", @"..", Path.DirectorySeparatorChar.ToString()), "out", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), destFileName);
                    if (!Directory.Exists(Path.GetDirectoryName(destFilePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destFilePath));
                    }
                    if (!File.Exists(destFilePath))
                    {
                        File.Move(filePath, destFilePath);
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region public virtual async Task MoveFileToOutDirectoryAsync(string filePath)
        /// <summary>
        /// Przenieś plik do katalogu z plikami po poprawnym wykonaniu operacji asynchronicznie
        /// Move the file to the directory with files after the correct operation asynchronously
        /// </summary>
        /// <param name="filePath">
        /// Ścieżka do pliku jako string
        /// File path as string
        /// </param>
        public virtual async Task MoveFileToOutDirectoryAsync(string filePath)
        {
            await Task.Run(() => {
                MoveFileToOutDirectory(filePath);
            });
        }
        #endregion

        #region public virtual void ReplaceFileToErrorDirectory(string filePath)
        /// <summary>
        /// Przenieś plik do katalogu z plikami po zarejestrowaniu błędu w migracji
        /// Replace the file to the files directory after logging the migration error
        /// </summary>
        /// <param name="filePath">
        /// Ścieżka do pliku jako string
        /// File path as string
        /// </param>
        public virtual void ReplaceFileToErrorDirectory(string filePath)
        {
            try
            {
                if (null != filePath && !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    string destFileName = Path.GetFileName(filePath);
                    string destinationBackupFileName = Path.Combine(Path.GetDirectoryName(filePath), string.Format("{0}{1}", @"..", Path.DirectorySeparatorChar.ToString()), "error", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), "backup", SetFileName(filePath));
                    string destFilePath = Path.Combine(Path.GetDirectoryName(filePath), string.Format("{0}{1}", @"..", Path.DirectorySeparatorChar.ToString()), "error", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), destFileName);
                    if (!Directory.Exists(Path.GetDirectoryName(destFilePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destFilePath));
                    }
                    if (!Directory.Exists(Path.GetDirectoryName(destinationBackupFileName)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationBackupFileName));
                    }
                    if (File.Exists(destFilePath))
                    {
                        File.Replace(filePath, destFilePath, destinationBackupFileName);
                    }
                    else
                    {
                        File.Move(filePath, destFilePath);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region public virtual async Task ReplaceFileToErrorDirectoryAsync(string filePath)
        /// <summary>
        /// Przenieś plik do katalogu z plikami po zarejestrowaniu błędu w migracji asynchronicznie
        /// Replace the file to the files directory after logging the migration error asynchronously
        /// </summary>
        /// <param name="filePath">
        /// Ścieżka do pliku jako string
        /// File path as string
        /// </param>
        public virtual async Task ReplaceFileToErrorDirectoryAsync(string filePath)
        {
            await Task.Run(() =>
            {
                ReplaceFileToErrorDirectory(filePath);
            });
        }
        #endregion

        #region public virtual void ReplaceFileToOutDirectory(string filePath)
        /// <summary>
        /// Przenieś plik do katalogu z plikami po poprawnym wykonaniu operacji
        /// Replace the file to the directory with files after the correct operation
        /// </summary>
        /// <param name="filePath">
        /// Ścieżka do pliku jako string
        /// File path as string
        /// </param>
        public virtual void ReplaceFileToOutDirectory(string filePath)
        {
            try
            {
                if (null != filePath && !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    string destFileName = Path.GetFileName(filePath);
                    string destinationBackupFileName = Path.Combine(Path.GetDirectoryName(filePath), string.Format("{0}{1}", @"..", Path.DirectorySeparatorChar.ToString()), "error", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), "backup", SetFileName(filePath));
                    string destFilePath = Path.Combine(Path.GetDirectoryName(filePath), string.Format("{0}{1}", @"..", Path.DirectorySeparatorChar.ToString()), "out", DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString(), destFileName);
                    if (!Directory.Exists(Path.GetDirectoryName(destFilePath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destFilePath));
                    }
                    if (!Directory.Exists(Path.GetDirectoryName(destinationBackupFileName)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationBackupFileName));
                    }
                    if (File.Exists(destFilePath))
                    {
                        File.Replace(filePath, destFilePath, destinationBackupFileName);
                    }
                    else
                    {
                        File.Move(filePath, destFilePath);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region public virtual async Task ReplaceFileToOutDirectoryAsync(string filePath)
        /// <summary>
        /// Przenieś plik do katalogu z plikami po poprawnym wykonaniu operacji asynchronicznie
        /// Replace the file to the directory with files after the correct operation asynchronously
        /// </summary>
        /// <param name="filePath">
        /// Ścieżka do pliku jako string
        /// File path as string
        /// </param>
        public virtual async Task ReplaceFileToOutDirectoryAsync(string filePath)
        {
            await Task.Run(() => {
                ReplaceFileToOutDirectory(filePath);
            });
        }
        #endregion
    }
    #endregion
}