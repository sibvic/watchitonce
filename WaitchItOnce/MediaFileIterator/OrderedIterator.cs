using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchItOnce;

namespace WatchItOnce.MediaFileIterator
{
    class OrderedIterator : IMediaFileIterator
    {
        public OrderedIterator(MediaFile[] files)
        {
            mFiles = files;
            mNextFile = 0;
        }

        MediaFile[] mFiles;
        int mNextFile;

        public MediaFile GetNextFile()
        {
            if (mNextFile >= mFiles.Length)
                return null;

            return mFiles[mNextFile++];
        }
    }
}
