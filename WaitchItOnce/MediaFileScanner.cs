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
using System.Text;
using System.IO;

namespace WatchItOnce
{
    class MediaFileScanner
    {
        MediaFileScanner(IFileFilter filter)
        {
            mFilter = filter;
        }
        IFileFilter mFilter;

        long getPosition(string path)
        {
            string infoPath = path + ".info";
            if (File.Exists(infoPath))
            {
                string data = File.ReadAllText(infoPath);
                long position;
                if (long.TryParse(data, out position))
                    return position;
            }
            return 0;
        }

        void scannFolder(string path)
        {
            try
            {
                foreach (string folder in Directory.GetDirectories((string)path))
                {
                    scannFolder(folder);
                }
                string[] extensions = new string[] { "*.mkv", "*.avi", "*.mp4", "*.webm", "*.wmv", "*.vob" };
                foreach (string extension in extensions)
                {
                    foreach (string file in Directory.GetFiles((string)path, extension))
                    {
                        if (mFilter != null && !mFilter.IsPassing(file))
                            continue;
                        long position = getPosition(file);
                        mFiles.Add(new MediaFile(file, position));
                    }
                }
            }
            catch (PathTooLongException)
            {
            }
        }

        List<MediaFile> mFiles = new List<MediaFile>();

        public static MediaFile[] GetFromFolder(string path, IFileFilter filter)
        {
            MediaFileScanner scaner = new MediaFileScanner(filter);
            scaner.scannFolder(path);
            return scaner.mFiles.ToArray();
        }
    }
}
