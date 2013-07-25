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

using WatchItOnce.FileFilter;

namespace WatchItOnce
{
    public class StringIterator
    {
        public StringIterator(string[] strings)
        {
            mCurrectString = 0;
            mStrings = strings;
        }

        string[] mStrings;
        int mCurrectString;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>null when runs out of strings</returns>
        public string GetNext()
        {
            if (mCurrectString >= mStrings.Length)
                return null;

            return mStrings[mCurrectString++];
        }
    }

    public class Options
    {
        public void Load(string[] args)
        {
            DeleteAfterWatch = false;
            RandomOrder = false;

            StringIterator strings = new StringIterator(args);
            string current = strings.GetNext();
            while (current != null)
            {
                switch (current)
                {
                    case "--filter":
                        current = strings.GetNext();
                        if (current == null)
                            throw new ArgumentException("--filter should be followed by filter string");
                        Filter = new SimpleMatchFilter(current);
                        break;
                    case "--delete":
                        DeleteAfterWatch = true;
                        break;
                    case "--random":
                        RandomOrder = true;
                        break;
                    default:
                        throw new NotSupportedException("Unknown argument:" + current);
                }
                current = strings.GetNext();
            }
        }

        public IFileFilter Filter { get; private set; }
        public bool DeleteAfterWatch { get; private set; }
        public bool RandomOrder { get; private set; }
    }
}
