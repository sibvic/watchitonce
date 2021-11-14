using System.IO;

namespace WatchItOnce.Core
{
    public class MediaFile
    {
        public MediaFile(string path)
        {
            Path = path;
        }

        static long GetPosition(string path)
        {
            string infoPath = path + ".info";
            if (File.Exists(infoPath))
            {
                string data = File.ReadAllText(infoPath);
                if (long.TryParse(data, out long position))
                {
                    return position;
                }
            }
            return 0;
        }

        public string Path { get; private set; }
        public long PositionSeconds
        {
            get { return GetPosition(Path); }
        }
    }
}
