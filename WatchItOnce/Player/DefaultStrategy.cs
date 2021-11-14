using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchItOnce.Player
{
    class DefaultStrategy : IStrategy
    {
        public void Dispose()
        {
            //do nothing
        }

        public void OnPaused()
        {
            //do nothing
        }

        public void OnStarted()
        {
            //do nothing
        }

        public void OnStopped()
        {
            //do nothing
        }
    }
}
