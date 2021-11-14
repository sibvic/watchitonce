//
//  Watch It Once
//  Copyright (C) 2017 Victor Tereschenko
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WatchItOnce.Core;

namespace WatchItOnce.MediaFileIterator
{
    class RandomIterator : IMediaFileIterator
    {
        public RandomIterator(MediaFile[] files, bool continueStartedFirst)
        {
            Random rnd = new Random(files.Length + DateTime.Now.Second);
            if (continueStartedFirst)
            {
                List<MediaFile> filesToContinue = new List<MediaFile>(files.Where(f => f.PositionSeconds > 3));
                List<MediaFile> filesToStart = new List<MediaFile>(files.Where(f => f.PositionSeconds <= 3));
                AddFiles(filesToContinue, rnd);
                AddFiles(filesToStart, rnd);
            }
            else
            {
                List<MediaFile> filesToRandomize = new List<MediaFile>(files);
                AddFiles(filesToRandomize, rnd);
            }
        }

        private void AddFiles(List<MediaFile> filesToRandomize, Random rnd)
        {
            while (filesToRandomize.Count > 0)
            {
                int next = rnd.Next(filesToRandomize.Count);
                MediaFile mediaFile = filesToRandomize[next];
                filesToRandomize.Remove(mediaFile);
                mFiles.Add(mediaFile);
            }
        }

        List<MediaFile> mFiles = new List<MediaFile>();

        public IEnumerator<MediaFile> GetEnumerator()
        {
            return mFiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mFiles.GetEnumerator();
        }
    }
}
