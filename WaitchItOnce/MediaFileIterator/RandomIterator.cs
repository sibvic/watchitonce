using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchItOnce;

namespace WatchItOnce.MediaFileIterator
{
    class RandomIterator : IMediaFileIterator
    {
        public RandomIterator(MediaFile[] files)
        {
            List<MediaFile> filesToRandomize = new List<MediaFile>(files);
            Random rnd = new Random(files.Length);
            while (filesToRandomize.Count > 0)
            {
                int next = rnd.Next(filesToRandomize.Count);
                MediaFile mediaFile = filesToRandomize[next];
                filesToRandomize.Remove(mediaFile);
                mFiles.Add(mediaFile);
            }
            mNextFile = 0;
        }

        List<MediaFile> mFiles = new List<MediaFile>();
        int mNextFile;

        public MediaFile GetNextFile()
        {
            if (mNextFile >= mFiles.Count)
                return null;

            return mFiles[mNextFile++];
        }
    }
}
