namespace WatchItOnce.Core
{
    public interface IFileFilter
    {
        bool IsPassing(string file);
    }
}
