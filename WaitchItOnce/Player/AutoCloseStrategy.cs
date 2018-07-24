using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WatchItOnce.Player
{
    /// <summary>
    /// Closes the player after certain amount of time.
    /// </summary>
    class AutoCloseStrategy : IStrategy
    {
        private readonly IPlayerWindowController controller;
        private Timer timer;

        public AutoCloseStrategy(IPlayerWindowController controller, int timeoutSeconds)
        {
            this.controller = controller;

            timer = new Timer
            {
                Interval = timeoutSeconds * 1000
            };
            timer.Elapsed += new ElapsedEventHandler(MTimer_Elapsed);
            timer.Start();
        }
        
        private void DeleteTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
        }

        void MTimer_Elapsed(object sender, ElapsedEventArgs e) => controller.DoClose();

        public void Dispose() => DeleteTimer();

        public void OnStopped()
        {
            //do nothing
        }

        public void OnStarted()
        {
            //do nothing
        }

        public void OnPaused()
        {
            //do nothing
        }
    }
}
