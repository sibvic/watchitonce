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
        public RandomIterator(MediaFile[] files, bool continueStartedFirst)
        {
            Random rnd = new Random(files.Length + DateTime.Now.Second);
            if (continueStartedFirst)
            {
                List<MediaFile> filesToContinue = new List<MediaFile>(files.Where(f => f.PositionSeconds > 3));
                List<MediaFile> filesToStart = new List<MediaFile>(files.Where(f => f.PositionSeconds <= 3));
                addFiles(filesToContinue, rnd);
                addFiles(filesToStart, rnd);
            }
            else
            {
                List<MediaFile> filesToRandomize = new List<MediaFile>(files);
                addFiles(filesToRandomize, rnd);
            }
        }

        private void addFiles(List<MediaFile> filesToRandomize, Random rnd)
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
