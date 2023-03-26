namespace WatchItOnce.Core
{
    public class MediaFileProvider
    {
        public static MediaFile[] GetFromFolder(string path, IFileFilter filter, string[] extensions)
        {
            var scaner = new MediaFileScanner(filter, extensions, new FileListProvider());
            scaner.ScannFolder(path);
            return scaner.Files.ToArray();
        }
    }
}
