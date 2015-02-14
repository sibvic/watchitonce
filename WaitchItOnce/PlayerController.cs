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
        public PlayerController(IDiskPlayer player, IMediaPlayerFactory playerFactory)
        {
            mPlayer = player;
            mPlayerFactory = playerFactory;
        }

        IDiskPlayer mPlayer;
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

        /// <summary>
        /// Switches deinterlacing mode in a loop.
        /// No deinterlacing
        /// Yadiff 2x
        /// Yadiff
        /// </summary>
        public void SwitchDeinterlacing()
        {
            if (mPlayer.Deinterlace.Enabled)
            {
                if (mPlayer.Deinterlace.Mode == DeinterlaceMode.yadif2x)
                    mPlayer.Deinterlace.Mode = DeinterlaceMode.yadif;
                else
                {
                    mPlayer.Deinterlace.Enabled = false;
                    mPlayer.Deinterlace.Mode = DeinterlaceMode.yadif2x;
                }
            }
            else
            {
                mPlayer.Deinterlace.Enabled = true;
                mPlayer.Deinterlace.Mode = DeinterlaceMode.yadif2x;
            }
        }

        /// <summary>
        /// Switches audio track in a loop
        /// </summary>
        public void SwitchAudioTrack()
        {
            int audioTracksCount = mPlayer.AudioTrackCount;
            if (audioTracksCount == 1)
                return;

            int currentAudioTrack = mPlayer.AudioTrack;
            int nextAudioTrack = mPlayer.AudioTrack + 1;
            if (nextAudioTrack > audioTracksCount)
                mPlayer.AudioTrack = 1;
            else
            {
                mPlayer.AudioTrack = nextAudioTrack;
                //HACK: RESOLVE LATER: sometime AudioTrackCount returns more tracks than it is actually is
                if (mPlayer.AudioTrack == currentAudioTrack)
                    mPlayer.AudioTrack = 1;
            }
            
        }
    }
}
