//
//  Watch It Once
//  Copyright (C) 2013 Victor Tereschenko
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//     
// ========================================================================

using CommandLine;
using System;
using System.Windows.Forms;
using WatchItOnce.Core;
using WatchItOnce.FileFilter;
using WatchItOnce.MediaFileIterator;

namespace WatchItOnce
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    (Options opts) => Execute(opts),
                    errs => 1);
            if (result != 0)
            {
                return;
            }
        }
        public static int Execute(Options options)
        {
            MediaFile[] files = GetFilesToPlay(options);
            if (files.Length == 0)
            {
                MessageBox.Show("No files to play");
                return -1;
            }

            IMediaFileIterator mediaFiles = CreateIterator(options, files);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var playerOptions = new PlayerOptions()
            {
                AutoNext = options.AutoNext,
                AutoClose = options.AutoClose,
                InitialVolume = options.Volume,
                Speed = options.Speed
            };
            var playerWindow = new PlayerWindow(mediaFiles, playerOptions);
            playerWindow.OnMediaSkipped += new OnMediaSkippedDelegate(PlayerWindow_OnMediaSkipped);
            playerWindow.OnMediaPaused += new OnMediaSkippedDelegate(PlayerWindow_OnMediaPaused);
            if (options.DeleteAfterWatch)
                playerWindow.OnMediaEnded += new OnMediaEndedDelegate(PlayerWindow_OnMediaEnded);
            playerWindow.OnLogMessage += PlayerWindow_OnLogMessage;
            MoveWindow(playerWindow, options.ScreenPosition);
            Application.Run(playerWindow);
            return 0;
        }

        private static MediaFile[] GetFilesToPlay(Options options)
        {
            if (string.IsNullOrEmpty(options.File))
            {
                return MediaFileProvider.GetFromFolder(System.IO.Directory.GetCurrentDirectory(), CreateFilter(options),
                    options.Extensions.Split(new char[] { ';' }));
            }
            return new[] { new MediaFile(options.File) };
        }

        private static void MoveWindow(PlayerWindow playerWindow, ScreenPosition screenPosition)
        {
            switch (screenPosition)
            {
                case ScreenPosition.BottomLeft:
                    playerWindow.Left = Screen.PrimaryScreen.WorkingArea.Left;
                    playerWindow.Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
                    playerWindow.Top = Screen.PrimaryScreen.WorkingArea.Height / 2;
                    playerWindow.Height = Screen.PrimaryScreen.WorkingArea.Height / 2;
                    break;
                case ScreenPosition.BottomRight:
                    playerWindow.Left = Screen.PrimaryScreen.WorkingArea.Width / 2;
                    playerWindow.Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
                    playerWindow.Top = Screen.PrimaryScreen.WorkingArea.Height / 2;
                    playerWindow.Height = Screen.PrimaryScreen.WorkingArea.Height / 2;
                    break;
                case ScreenPosition.TopLeft:
                    playerWindow.Left = Screen.PrimaryScreen.WorkingArea.Left;
                    playerWindow.Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
                    playerWindow.Top = Screen.PrimaryScreen.WorkingArea.Top;
                    playerWindow.Height = Screen.PrimaryScreen.WorkingArea.Height / 2;
                    break;
                case ScreenPosition.TopRight:
                    playerWindow.Left = Screen.PrimaryScreen.WorkingArea.Width / 2;
                    playerWindow.Width = Screen.PrimaryScreen.WorkingArea.Width / 2;
                    playerWindow.Top = Screen.PrimaryScreen.WorkingArea.Top;
                    playerWindow.Height = Screen.PrimaryScreen.WorkingArea.Height / 2;
                    break;
            }
        }

        private static IFileFilter CreateFilter(Options options)
        {
            if (!string.IsNullOrEmpty(options.Filter))
            {
                return new SimpleMatchFilter(options.Filter);
            }
            if (!string.IsNullOrEmpty(options.RegexpFilterString))
            {
                return new RegexpMatchFilter(options.RegexpFilterString);
            }
            return null;
        }

        static string FormatPosition(long position)
        {
            var hours = position / 3600;
            position -= hours * 3600;
            var minutes = position / 60;
            position -= minutes * 60;
            return hours.ToString("00") + ":" + minutes.ToString("00") + ":" + position.ToString("00");
        }

        private static void PlayerWindow_OnLogMessage(MediaFile file, string message, long position)
        {
            string logPath = file.Path + ".desc";
            System.IO.File.AppendAllText(logPath, FormatPosition(position) + "\n" + message + "\n\n");
        }

        private static IMediaFileIterator CreateIterator(Options options, MediaFile[] files)
        {
            switch (options.SortOrder)
            {
                default:
                case SortOrder.Default:
                    return new OrderedIterator(files);
                case SortOrder.Random:
                    return new RandomIterator(files, false);
                case SortOrder.RandomContinue:
                    return new RandomIterator(files, true);
                case SortOrder.ByName:
                    return new SoredByNameIterator(files);
            }
        }

        static void PlayerWindow_OnMediaEnded(MediaFile file)
        {
            string infoPath = file.Path + ".info";
            if (System.IO.File.Exists(infoPath))
                System.IO.File.Delete(infoPath);

            System.IO.FileAttributes attrs = System.IO.File.GetAttributes(file.Path);
            if (attrs.HasFlag(System.IO.FileAttributes.ReadOnly))
                System.IO.File.SetAttributes(file.Path, System.IO.FileAttributes.Normal);
            System.IO.File.Delete(file.Path);
        }

        static void PlayerWindow_OnMediaSkipped(MediaFile file, long lastPosition)
        {
            SaveLocation(file, lastPosition);
        }

        private static void SaveLocation(MediaFile file, long lastPosition)
        {
            string infoPath = file.Path + ".info";
            System.IO.File.WriteAllText(infoPath, lastPosition.ToString());
        }

        static void PlayerWindow_OnMediaPaused(MediaFile file, long lastPosition)
        {
            SaveLocation(file, lastPosition);
        }
    }
}
