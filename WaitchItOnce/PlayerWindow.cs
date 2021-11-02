using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Implementation;
using Declarations;
using Declarations.Players;
using System.Timers;
using Microsoft.WindowsAPICodePack.Taskbar;
using WatchItOnce.Player;
using WatchItOnce.Forms;

namespace WatchItOnce
{
    public delegate void OnMediaEndedDelegate(MediaFile file);
    public delegate void OnLogMessageDelegate(MediaFile file, string message, long position);
    public delegate void OnMediaSkippedDelegate(MediaFile file, long lastPosition);

    public partial class PlayerWindow : Form, IPlayerWindowController
    {
        public PlayerWindow(IMediaFileIterator files, PlayerOptions options)
        {
            mProgressTimer = new System.Timers.Timer(500);
            mProgressTimer.Elapsed += new ElapsedEventHandler(UpdateProgress);
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

            _playPauseButton = new ThumbnailToolBarButton(Properties.Resources.PlayIcon, "Play/Pause");//todo: use custom icons
            _playPauseButton.Click += _playPauseButton_Click;
            _nextButton = new ThumbnailToolBarButton(Properties.Resources.NextIcon, "Next");
            _nextButton.Click += _nextButton_Click;
            _markWatchedButton = new ThumbnailToolBarButton(Properties.Resources.DeleteIcon, "Mark as watched");
            _markWatchedButton.Click += _markWatchedButton_Click;
            TaskbarManager.Instance.ThumbnailToolBars.AddButtons(Handle, new ThumbnailToolBarButton[]
            {
                _playPauseButton,
                _nextButton,
                _markWatchedButton
            });

            if (mOptions.AutoNext.HasValue)
            {
                strategy = new AutoNextStrategy(this, mOptions.AutoNext.Value);
            }
            else if (mOptions.AutoClose.HasValue)
            {
                strategy = new AutoCloseStrategy(this, mOptions.AutoClose.Value);
            }
            else
            {
                strategy = new DefaultStrategy();
            }
        }

        private void _markWatchedButton_Click(object sender, ThumbnailButtonClickedEventArgs e) => DoMarkWatched(false);
        private void _nextButton_Click(object sender, ThumbnailButtonClickedEventArgs e) => DoNext();
        private void _playPauseButton_Click(object sender, ThumbnailButtonClickedEventArgs e) => DoPlayPause();

        IStrategy strategy;

        public event OnMediaEndedDelegate OnMediaEnded;
        public event OnMediaSkippedDelegate OnMediaSkipped;
        public event OnLogMessageDelegate OnLogMessage;

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
        System.Timers.Timer mProgressTimer;

        string _mediaName;
        long? _lastPosition;

        private ThumbnailToolBarButton _playPauseButton;
        private ThumbnailToolBarButton _nextButton;
        private ThumbnailToolBarButton _markWatchedButton;

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
            MediaFile next = GetNextFile();
            if (next == null)
            {
                mPlayingFile = null;
                return false;
            }
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

        /// <summary>
        /// Get next file in the queue.
        /// Unexisted files will be skipped.
        /// </summary>
        /// <returns>Next file or null</returns>
        private MediaFile GetNextFile()
        {
            while (mFilesIterator.MoveNext())
            {
                MediaFile next = mFilesIterator.Current;
                if (next == null || System.IO.File.Exists(next.Path))
                {
                    return next;
                }
            }

            return null;
        }

        #region speed control
        float mSpeed = 1.0f;
        private void SpeedOnOff()
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
                case Keys.Z:
                    AddMark();
                    break;
                case Keys.A:
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
                    DoNext();
                    break;
                case Keys.M:
                    DoMarkWatched(true);
                    break;
                case Keys.D0:
                    SpeedOnOff();
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
                    DoPlayPause();
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

        private void AddMark()
        {
            DoPlayPause();
            var messageForm = new AskMessageForm();
            var result = messageForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                var message = messageForm.GetMessage();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    OnLogMessage?.Invoke(mPlayingFile, message, (long)(mPlayer.Length * mPlayer.Position / 1000));
                }
            }
            DoPlayPause();
        }

        MediaFile _lastFileToDelete;
        private void DoMarkWatched(bool askConfirmation)
        {
            if (askConfirmation)
            {
                if (MessageBox.Show("Are you sure?", "Mark as watched?", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
            }
            else
            {
                if (_lastFileToDelete != mPlayingFile)
                {
                    _lastFileToDelete = mPlayingFile;
                    return;
                }
            }
            mPlayerController.Stop();
            if (OnMediaEnded != null)
            {
                OnMediaEnded(mPlayingFile);
                mPlayingFile = null;
            }
            PlayNextVideo();
        }
        
        private void DoPlayPause()
        {
            if (mPlayerController.IsPlaying)
                mPlayerController.Pause();
            else
                mPlayerController.Play();
        }

        void Events_PlayerStopped(object sender, EventArgs e) => strategy.OnStopped();
        void Events_PlayerPlaying(object sender, EventArgs e) => strategy.OnStarted();
        void Events_PlayerPaused(object sender, EventArgs e) => strategy.OnPaused();

        void Events_MediaEnded(object sender, EventArgs e)
        {
            strategy.OnStarted();
            mPlayerController.Stop();
            if (OnMediaEnded != null)
            {
                OnMediaEnded(mPlayingFile);
                mPlayingFile = null;
            }
            if (!PlayNextVideo())
                mPlayerController.Pause();
        }

        private void PlayerWindow_Load(object sender, EventArgs e) => PlayNextVideo();

        private void PlayerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            strategy.Dispose();
            if (OnMediaSkipped != null && mPlayingFile != null)
                OnMediaSkipped(mPlayingFile, (long)(mPlayer.Length * mPlayer.Position / 1000));
            mProgressTimer.Stop();
            mProgressTimer.Dispose();
        }

        public void DoNext()
        {
            if (!PlayNextVideo())
                mPlayerController.Pause();
        }

        public void DoClose()
        {
            BeginInvoke(new Action(delegate
            {
                Close();
            }));
        }
    }
}
