using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WatchItOnce.Player
{
    /// <summary>
    /// Automatically moves to the next item after timeout.
    /// </summary>
    class AutoNextStrategy : IStrategy
    {
        private readonly IPlayerWindowController controller;
        private readonly int timeoutSeconds;
        private Timer timer;

        public AutoNextStrategy(IPlayerWindowController controller, int timeoutSeconds)
        {
            this.controller = controller;
            this.timeoutSeconds = timeoutSeconds;
        }

        private void CreateTimer()
        {
            DeleteTimer();
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

        void MTimer_Elapsed(object sender, ElapsedEventArgs e) => controller.DoNext();

        public void OnStopped() => DeleteTimer();
        public void OnStarted() => CreateTimer();
        public void OnPaused() => DeleteTimer();
        public void Dispose() => DeleteTimer();

    }
}
