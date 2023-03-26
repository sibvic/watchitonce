using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WatchItOnce.Core
{

    public class MediaFileScanner
    {
        public MediaFileScanner(IFileFilter filter, string[] extensions, IFileListProvider fileListProvider)
        {
            this.filter = filter;
            this.extensions = extensions;
            this.fileListProvider = fileListProvider;
        }

        readonly IFileFilter filter;
        readonly string[] extensions;
        private readonly IFileListProvider fileListProvider;

        public void ScannFolder(string path)
        {
            try
            {
                foreach (string folder in fileListProvider.GetDirectories(path))
                {
                    ScannFolder(folder);
                }
                foreach (string extension in extensions.Distinct())
                {
                    foreach (string file in fileListProvider.GetFiles(path, extension))
                    {
                        if (filter != null && !filter.IsPassing(file))
                            continue;
                        Files.Add(new MediaFile(file));
                    }
                }
            }
            catch (PathTooLongException)
            {
            }
        }

        public List<MediaFile> Files { get; } = new List<MediaFile>();
    }
}
