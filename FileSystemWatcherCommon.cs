using System;
using System.IO;
using System.Reflection;

namespace FileSystemWatcherCommon
{
    #region public abstract class FileSystemWatcherCommon : IFileSystemWatcherCommon
    /// <summary>
    /// Abstrakcyjna, wspólna klasa obserwacji zdarzeń plików w katalogu
    /// Abstract common class for observing files in a directory
    /// </summary>
    public abstract class FileSystemWatcherCommon : IFileSystemWatcherCommon
    {
        #region private static readonly log4net.ILog log4net
        /// <summary>
        /// Log4net Logger
        /// Log4net Logger
        /// </summary>
        private static readonly log4net.ILog log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

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

        #region public virtual void Watch
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
                    fileSystemWatcher.NotifyFilter = notifyFilters;
                    if (null != filter && !string.IsNullOrWhiteSpace(filter))
                    {
                        fileSystemWatcher.Filter = filter;// "*.txt";
                    }
                    fileSystemWatcher.Created += OnCreated;
                    fileSystemWatcher.Changed += OnChanged;
                    fileSystemWatcher.Renamed += OnRenamed;
                    fileSystemWatcher.Deleted += OnDeleted;
                    fileSystemWatcher.Error += OnError;
                    fileSystemWatcher.EnableRaisingEvents = true;
                    while (true)
                    {
                        ;
                    }
                }
                else
                {
                    log4net.Info($"Directory { path } do not exists!");
                }
            }
        }
        #endregion

        public virtual void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed.
            log4net.Info($"OnChanged file: {e.FullPath} {e.ChangeType}");
            OnChangedEventHandler?.Invoke(source, e);
        }

        public virtual void OnCreated(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is created.
            log4net.Info($"OnCreated file: {e.FullPath} {e.ChangeType}");
            OnCreatedEventHandler?.Invoke(source, e);
        }

        public virtual void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            log4net.Info($"OnRenamed file: {e.OldFullPath} renamed to {e.FullPath}");
            OnRenamedEventHandler?.Invoke(source, e);
        }

        public virtual void OnDeleted(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed.
            log4net.Info($"OnDelete file: {e.FullPath} {e.ChangeType}");
            OnDeletedEventHandler?.Invoke(source, e);
        }

        public virtual void OnError(object source, ErrorEventArgs e)
        {
            // Specify what is error.
            log4net.Error($"Error { e.GetException().Message }, { e.GetException().StackTrace }", e.GetException());
            OnErrorEventHandler?.Invoke(source, e);
        }
    }
    #endregion
}