using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Implementation;
using Declarations;
using Declarations.Players;
using System.Timers;

namespace WatchItOnce
{
    public delegate void OnMediaEndedDelegate(MediaFile file);
    public delegate void OnMediaSkippedDelegate(MediaFile file, long lastPosition);

    public partial class PlayerWindow : Form
    {
        public PlayerWindow(IMediaFileIterator files, PlayerOptions options)
        {
            mProgressTimer = new System.Timers.Timer(500);
            mProgressTimer.Elapsed += new System.Timers.ElapsedEventHandler(UpdateProgress);
            mProgressTimer.Start();

            mOptions = options;
            mFiles = files;
            mFilesIterator = mFiles.GetEnumerator();
            InitializeComponent();

            var playerPath = Application.StartupPath;
            mPlayerFactory = new MediaPlayerFactory(playerPath);
            mPlayer = mPlayerFactory.CreatePlayer<IDiskPlayer>();
            mPlayerController = new PlayerController(mPlayer, mPlayerFactory);

            mPlayer.Events.MediaEnded += new EventHandler(Events_MediaEnded);
            mPlayer.Events.PlayerStopped += new EventHandler(Events_PlayerStopped);
            mPlayer.Events.PlayerPaused += new EventHandler(Events_PlayerPaused);
            mPlayer.Events.PlayerPlaying += new EventHandler(Events_PlayerPlaying);

            mPlayer.WindowHandle = Handle;
            mPlayer.KeyInputEnabled = false;

            KeyDown += new KeyEventHandler(PlayerWindow_KeyDown);
        }

        public event OnMediaEndedDelegate OnMediaEnded;
        public event OnMediaSkippedDelegate OnMediaSkipped;

        PlayerOptions mOptions;
        IMediaFileIterator mFiles;
        IEnumerator<MediaFile> mFilesIterator;

        IMediaPlayerFactory mPlayerFactory;
        IDiskPlayer mPlayer;
        PlayerController mPlayerController;
        int mLastWidth;
        int mLastHeight;
        int mLastTop;
        int mLastLeft;
        MediaFile mPlayingFile;
        System.Timers.Timer mTimer;
        System.Timers.Timer mProgressTimer;

        string _mediaName;
        long? _lastPosition;

        private void UpdateProgress(object sender, ElapsedEventArgs e)
        {
            if (mPlayingFile != null && mPlayerController.IsPlaying)
            {
                if (mPlayer.Length > 0)
                {
                    long positionInSeconds = mPlayer.Time / 1000;
                    if (_lastPosition.HasValue && positionInSeconds == _lastPosition.Value)
                        return;
                    _lastPosition = positionInSeconds;
                    var position = mPlayer.Time * 100.0 / mPlayer.Length;
                    var cationTime = string.Format(" {0}% ({1}/{2})", Math.Round(position), TimeSpan.FromMilliseconds(mPlayer.Time).ToString(@"hh\:mm\:ss"), TimeSpan.FromMilliseconds(mPlayer.Length).ToString(@"hh\:mm\:ss"));
                    BeginInvoke(new Action(delegate
                    {
                        Text = _mediaName + cationTime;
                    }));
                }
            }
        }

        private void EnterFullscreen()
        {
            mLastWidth = Width;
            mLastHeight = Height;
            mLastTop = Top;
            mLastLeft = Left;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }

        private void EscapeFullscreen()
        {
            WindowState = FormWindowState.Normal;
            FormBorderStyle = FormBorderStyle.Sizable;
            Width = mLastWidth;
            Height = mLastHeight;
            Top = mLastTop;
            Left = mLastLeft;
        }

        void Open(MediaFile file)
        {
            mPlayingFile = file;

            string fullFileName = mPlayingFile.Path;
            if (mPlayerController.IsPlaying)
                mPlayerController.Stop();
            mPlayerController.Open(fullFileName);
            _mediaName = System.IO.Path.GetFileNameWithoutExtension(mPlayingFile.Path);
            Text = _mediaName;
        }

        private bool PlayNextVideo()
        {
            if (!mFilesIterator.MoveNext())
            {
                mPlayingFile = null;
                return false;
            }
            MediaFile next = mFilesIterator.Current;
            if (mPlayingFile != null && OnMediaSkipped != null)
                OnMediaSkipped(mPlayingFile, (long)(mPlayer.Length * mPlayer.Position / 1000));

            BeginInvoke(new Action(delegate
            {
                int vol = mPlayer.Volume;
                Open(next);
                mPlayerController.Play();
                mPlayerController.Seek(next.PositionSeconds * 1000);
                mPlayer.Volume = vol;
            }));
            return true;
        }

        #region speed control
        float mSpeed = 1.0f;
        private void speedOnOff()
        {
            if (mPlayer.PlaybackRate == 1.0f)
                mPlayer.PlaybackRate = mSpeed;
            else
            {
                mSpeed = mPlayer.PlaybackRate;
                mPlayer.PlaybackRate = 1.0f;
            }
        }
        #endregion

        private void SeekSeconds(int seconds)
        {
            mPlayer.Time = Math.Min(mPlayer.Length, Math.Max(0, mPlayer.Time + seconds * 1000));
        }

        private static int GetSeekSeconds()
        {
            int seek = 5;
            if ((ModifierKeys & Keys.Alt) != 0)
                seek *= 2;
            if ((ModifierKeys & Keys.Control) != 0)
                seek *= 3;
            if ((ModifierKeys & Keys.Shift) != 0)
                seek *= 4;
            return seek;
        }

        private static int GetVolumeLeap()
        {
            int gain = 1;
            if ((ModifierKeys & Keys.Alt) != 0)
                gain *= 2;
            if ((ModifierKeys & Keys.Control) != 0)
                gain *= 3;
            if ((ModifierKeys & Keys.Shift) != 0)
                gain *= 4;
            return gain;
        }

        void PlayerWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    //TODO: test
                    mPlayerController.SwitchDeinterlacing();
                    break;
                case Keys.S:
                    mPlayerController.SwitchAudioTrack();
                    break;
                case Keys.Enter:
                    if (WindowState == FormWindowState.Maximized)
                        EscapeFullscreen();
                    else
                        EnterFullscreen();
                    break;
                case Keys.B:
                    if (mPlayerController.IsPlaying)
                    {
                        mPlayerController.Pause();
                        EscapeFullscreen();
                    }
                    e.Handled = true;
                    break;
                case Keys.N:
                    if (!PlayNextVideo())
                        mPlayerController.Pause();
                    break;
                case Keys.M:
                    if (MessageBox.Show("Are you sure?", "Mark as watched?", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                        break;
                    mPlayerController.Stop();
                    if (OnMediaEnded != null)
                    {
                        OnMediaEnded(mPlayingFile);
                        mPlayingFile = null;
                    }
                    PlayNextVideo();
                    break;
                case Keys.D0:
                    speedOnOff();
                    break;
                case Keys.D1:
                    mPlayer.PlaybackRate = 1.1f;
                    break;
                case Keys.D2:
                    mPlayer.PlaybackRate = 1.2f;
                    break;
                case Keys.D3:
                    mPlayer.PlaybackRate = 1.3f;
                    break;
                case Keys.D4:
                    mPlayer.PlaybackRate = 1.4f;
                    break;
                case Keys.D5:
                    mPlayer.PlaybackRate = 1.5f;
                    break;
                case Keys.D6:
                    mPlayer.PlaybackRate = 1.6f;
                    break;
                case Keys.D7:
                    mPlayer.PlaybackRate = 1.7f;
                    break;
                case Keys.D8:
                    mPlayer.PlaybackRate = 1.8f;
                    break;
                case Keys.D9:
                    mPlayer.PlaybackRate = 1.9f;
                    break;
                case Keys.Escape:
                    if (WindowState == FormWindowState.Maximized)
                        EscapeFullscreen();
                    break;
                case Keys.Space:
                    if (mPlayerController.IsPlaying)
                        mPlayerController.Pause();
                    else
                        mPlayerController.Play();
                    break;
                case Keys.Left:
                    {
                        int seek = GetSeekSeconds();
                        SeekSeconds(-seek);
                    }
                    e.Handled = true;
                    break;
                case Keys.Right:
                    {
                        int seek = GetSeekSeconds();
                        SeekSeconds(seek);
                    }
                    e.Handled = true;
                    break;
                case Keys.Up:
                    {
                        int gain = GetVolumeLeap();
                        mPlayer.Volume = Math.Min(100, mPlayer.Volume + gain);
                    }
                    e.Handled = true;
                    break;
                case Keys.Down:
                    {
                        int gain = GetVolumeLeap();
                        mPlayer.Volume = Math.Max(0, mPlayer.Volume - gain);
                    }
                    e.Handled = true;
                    break;
            }
        }

        void Events_PlayerStopped(object sender, EventArgs e)
        {
            DeleteTimer();
        }

        void Events_PlayerPlaying(object sender, EventArgs e)
        {
            DeleteTimer();
            if (mOptions.AutoNext != null)
            {
                mTimer = new System.Timers.Timer();
                mTimer.Interval = mOptions.AutoNext.Value * 1000;
                mTimer.Elapsed += new System.Timers.ElapsedEventHandler(MTimer_Elapsed);
                mTimer.Start();
            }
        }

        void MTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            PlayNextVideo();
        }

        void Events_PlayerPaused(object sender, EventArgs e)
        {
            DeleteTimer();
        }

        private void DeleteTimer()
        {
            if (mTimer != null)
            {
                mTimer.Stop();
                mTimer.Dispose();
                mTimer = null;
            }
        }

        void Events_MediaEnded(object sender, EventArgs e)
        {
            DeleteTimer();
            mPlayerController.Stop();
            if (OnMediaEnded != null)
            {
                OnMediaEnded(mPlayingFile);
                mPlayingFile = null;
            }
            if (!PlayNextVideo())
                mPlayerController.Pause();
        }

        private void PlayerWindow_Load(object sender, EventArgs e)
        {
            PlayNextVideo();
        }

        private void PlayerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (OnMediaSkipped != null && mPlayingFile != null)
                OnMediaSkipped(mPlayingFile, (long)(mPlayer.Length * mPlayer.Position / 1000));
            mProgressTimer.Stop();
            mProgressTimer.Dispose();
        }
    }
}
