using System.Collections.Generic;
using System.IO;

namespace WatchItOnce.Core
{
    public class MediaFileScanner
    {
        MediaFileScanner(IFileFilter filter, string[] extensions)
        {
            this.filter = filter;
            this.extensions = extensions;
        }

        readonly IFileFilter filter;
        readonly string[] extensions;

        void ScannFolder(string path)
        {
            try
            {
                foreach (string folder in Directory.GetDirectories(path))
                {
                    ScannFolder(folder);
                }
                foreach (string extension in extensions)
                {
                    foreach (string file in Directory.GetFiles(path, extension))
                    {
                        if (filter != null && !filter.IsPassing(file))
                            continue;
                        files.Add(new MediaFile(file));
                    }
                }
            }
            catch (PathTooLongException)
            {
            }
        }

        readonly List<MediaFile> files = new List<MediaFile>();

        public static MediaFile[] GetFromFolder(string path, IFileFilter filter, string[] extensions)
        {
            MediaFileScanner scaner = new MediaFileScanner(filter, extensions);
            scaner.ScannFolder(path);
            return scaner.files.ToArray();
        }
    }
}
