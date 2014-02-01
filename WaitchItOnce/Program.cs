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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using WatchItOnce;
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
            Options options = new Options();
            try
            {
                options.Load(args);
            }
            catch (ArgumentException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return;
            }
            catch (NotSupportedException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return;
            }

            MediaFile[] files = MediaFileScanner.GetFromFolder(System.IO.Directory.GetCurrentDirectory(), options.Filter,
                options.Extensions.ToArray());
            if (files.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("No files to play");
                return;
            }

            IMediaFileIterator mediaFiles;
            if (options.RandomOrder)
                mediaFiles = new RandomIterator(files);
            else
                mediaFiles = new OrderedIterator(files);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            PlayerOptions playerOptions = new PlayerOptions(options.AutoNext);
            var playerWindow = new PlayerWindow(mediaFiles, playerOptions);
            playerWindow.OnMediaSkipped += new OnMediaSkippedDelegate(playerWindow_OnMediaSkipped);
            if (options.DeleteAfterWatch)
                playerWindow.OnMediaEnded += new OnMediaEndedDelegate(playerWindow_OnMediaEnded);
            Application.Run(playerWindow);
        }

        static void playerWindow_OnMediaEnded(MediaFile file)
        {
            string infoPath = file.Path + ".info";
            if (System.IO.File.Exists(infoPath))
                System.IO.File.Delete(infoPath);

            System.IO.FileAttributes attrs = System.IO.File.GetAttributes(file.Path);
            if (attrs.HasFlag(System.IO.FileAttributes.ReadOnly))
                System.IO.File.SetAttributes(file.Path, System.IO.FileAttributes.Normal);
            System.IO.File.Delete(file.Path);
        }

        static void playerWindow_OnMediaSkipped(MediaFile file, long lastPosition)
        {
            string infoPath = file.Path + ".info";
            System.IO.File.WriteAllText(infoPath, lastPosition.ToString());
        }
    }
}
