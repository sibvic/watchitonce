namespace WatchItOnce.Core
{
    public interface IFileListProvider
    {
        string[] GetDirectories(string path);
        string[] GetFiles(string path, string extension);
    }
}
