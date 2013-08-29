using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Declarations.Players;
using Declarations;
using Declarations.Media;
using Declarations.Events;

namespace WatchItOnce
{
    class PlayerController
    {
        public PlayerController(IVideoPlayer player, IMediaPlayerFactory playerFactory)
        {
            mPlayer = player;
            mPlayerFactory = playerFactory;
        }

        IVideoPlayer mPlayer;
        IMediaPlayerFactory mPlayerFactory;
        IMedia mMedia;

        public void Open(string file)
        {
            mMedia = mPlayerFactory.CreateMedia<IMedia>(file, new string[] { });
            mMedia.Events.StateChanged += MediaStateChange;
            mPlayer.Open(mMedia);
            mMedia.Parse(true);
        }

        public EventHandler<MediaStateChange> MediaStateChange;

        public bool IsPlaying { get; private set; }

        public void Seek(long milliseconds)
        {
            mPlayer.Time = milliseconds;
        }

        public void Stop()
        {
            if (mMedia == null)
                return;

            IsPlaying = false;
            mPlayer.Stop();
            mMedia.Dispose();
        }

        public void Pause()
        {
            if (!IsPlaying)
                return;

            IsPlaying = false;
            mPlayer.Pause();
        }

        public void Play()
        {
            if (IsPlaying)
                return;

            IsPlaying = true;
            mPlayer.Play();
        }
    }
}
