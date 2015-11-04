using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchItOnce;

namespace WatchItOnce.MediaFileIterator
{
    class OrderedIterator : IMediaFileIterator
    {
        List<MediaFile> mFiles = new List<MediaFile>();
        public OrderedIterator(MediaFile[] files)
        {
            mFiles.AddRange(files);
        }

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
