using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchItOnce.Player
{
    interface IPlayerWindowController
    {
        /// <summary>
        /// Moves to the next item.
        /// </summary>
        void DoNext();

        /// <summary>
        /// Closes player window
        /// </summary>
        void DoClose();
    }
}
