using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WatchItOnce.FileFilter
{
    class RegexpMatchFilter : IFileFilter
    {
        public RegexpMatchFilter(string regexp)
        {
            mFilter = new Regex(regexp);
        }

        Regex mFilter;

        public bool IsPassing(string file)
        {
            return mFilter.IsMatch(file);
        }
    }
}
