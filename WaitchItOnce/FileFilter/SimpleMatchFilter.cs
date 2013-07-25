using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WatchItOnce;

namespace WatchItOnce.FileFilter
{
    class SimpleMatchFilter : IFileFilter
    {
        public SimpleMatchFilter(string text)
        {
            mText = text.ToLower();
        }

        string mText;

        public bool IsPassing(string file)
        {
            return System.IO.Path.GetFileNameWithoutExtension(file).ToLower().Contains(mText);
        }
    }
}
