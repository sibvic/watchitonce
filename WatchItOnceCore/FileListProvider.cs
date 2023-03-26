using System.IO;

namespace WatchItOnce.Core
{
    class FileListProvider : IFileListProvider
    {
        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public string[] GetFiles(string path, string extension)
        {
            return Directory.GetFiles(path, extension);
        }
    }
}
