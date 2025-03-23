namespace FolderMonitor.Server.Helpers
{
    public class DirectoryWatcherHelper
    {
        private FileSystemWatcher _fileSystemWatcher;
        private string _directoryToWatch;
        public event EventHandler<FileSystemEventArgs> OnNewFileCreated;

        public DirectoryWatcherHelper(string directoryPath)
        {
            _directoryToWatch = directoryPath;

            // Initialize FileSystemWatcher to watch the directory for new files
            _fileSystemWatcher = new FileSystemWatcher
            {
                Path = _directoryToWatch,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                Filter = "*.*",  // Watch all files
                EnableRaisingEvents = true // Enable event detection
            };

            _fileSystemWatcher.Created += OnCreated;
        }
        // Event handler when a new file is created
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            OnNewFileCreated?.Invoke(this, e);
        }

        public void Start()
        {
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
        }
    }
}
