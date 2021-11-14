using WatchItOnce.Core;

namespace WatchItOnce.Manager
{
    class NoFilter : IFileFilter
    {
        public bool IsPassing(string file)
        {
            return true;
        }
    }
}
