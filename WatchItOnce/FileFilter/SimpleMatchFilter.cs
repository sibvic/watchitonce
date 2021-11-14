using WatchItOnce.Core;

namespace WatchItOnce.FileFilter
{
    class SimpleMatchFilter : IFileFilter
    {
        public SimpleMatchFilter(string text)
        {
            this.text = text.ToLower();
        }

        string text;

        public bool IsPassing(string file)
        {
            return System.IO.Path.GetFileNameWithoutExtension(file).ToLower().Contains(text);
        }
    }
}
