using System;

namespace WatchItOnce.Player
{
    /// <summary>
    /// Behavior strategy
    /// </summary>
    interface IStrategy : IDisposable
    {
        void OnStopped();
        void OnStarted();
        void OnPaused();
    }
}
