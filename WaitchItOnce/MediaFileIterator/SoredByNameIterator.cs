using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchItOnce.MediaFileIterator
{
    class SoredByNameIterator : IMediaFileIterator
    {
        List<MediaFile> mFiles = new List<MediaFile>();
        public SoredByNameIterator(MediaFile[] files)
        {
            mFiles.AddRange(files);
            mFiles.Sort(delegate (MediaFile left, MediaFile right)
            {
                var leftName = System.IO.Path.GetFileNameWithoutExtension(left.Path);
                var rightname = System.IO.Path.GetFileNameWithoutExtension(right.Path);
                return leftName.CompareTo(rightname);
            });
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
