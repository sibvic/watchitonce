using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchItOnce
{
    /// <summary>
    /// Options for the player.
    /// </summary>
    public class PlayerOptions
    {
        public PlayerOptions(int? autoNext)
        {
            AutoNext = autoNext;
        }

        public int? AutoNext { get; private set; }
    }
}
