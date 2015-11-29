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

    public partial class Options
    {
        public Options()
        {
            mOptions.Add(new AutoNextOptionController(this));
        }
        List<IOptionController> mOptions = new List<IOptionController>();

        bool parseOption(string optionName, StringIterator strings)
        {
            foreach (IOptionController option in mOptions)
            {
                if (option.Parse(optionName, strings))
                    return true;
            }
            return false;
        }

        public void Load(string[] args)
        {
            foreach (IOptionController option in mOptions)
            {
                option.ResetToDefault();
            }

            DeleteAfterWatch = false;
            SortOrder = SortOrder.Default;
            Extensions = new List<string>(new string[] { "*.mkv", "*.avi", "*.mp4", "*.webm", "*.wmv", "*.vob", "*.ts", "*.mpg" });

            StringIterator strings = new StringIterator(args);
            string current = strings.GetNext();
            while (current != null)
            {
                if (!parseOption(current, strings))
                {
                    switch (current)
                    {
                        case "--filter":
                            current = strings.GetNext();
                            if (current == null)
                                throw new ArgumentException("--filter should be followed by filter string");
                            Filter = new SimpleMatchFilter(current);
                            break;
                        case "--rfilter":
                            current = strings.GetNext();
                            if (current == null)
                                throw new ArgumentException("--rfilter should be followed by filter string");
                            Filter = new RegexpMatchFilter(current);
                            break;
                        case "--delete":
                            DeleteAfterWatch = true;
                            break;
                        case "--random":
                            SortOrder = SortOrder.Random;
                            break;
                        case "--sort-by-name":
                            SortOrder = SortOrder.ByName;
                            break;
                        case "--extensions":
                            current = strings.GetNext();
                            if (current == null)
                                throw new ArgumentException("--extensions should be followed by list of extension separated by ;");
                            string[] extensions = current.Split(new char[] { ';' });
                            Extensions = new List<string>(extensions);
                            break;
                        default:
                            throw new NotSupportedException("Unknown argument:" + current);
                    }
                }
                current = strings.GetNext();
            }
        }

        public IFileFilter Filter { get; private set; }
        public bool DeleteAfterWatch { get; private set; }
        public SortOrder SortOrder { get; private set; }
        public List<string> Extensions { get; private set; }
        public int? AutoNext { get; internal set; }
    }
}
