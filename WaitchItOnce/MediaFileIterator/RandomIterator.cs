using System;
using System.Collections;
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
            Random rnd = new Random(files.Length + DateTime.Now.Second);
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
