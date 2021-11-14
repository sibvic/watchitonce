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

using CommandLine;
using System;
using System.Collections.Generic;

using WatchItOnce.FileFilter;

namespace WatchItOnce
{
    [Verb("play", HelpText = "Play files.")]
    public class Options
    {
        [Option("filter", Required = false, HelpText = "Filter.")]
        public string Filter{ get; set; }
        [Option("rfilter", Required = false, HelpText = "Regexp filter.")]
        public string RegexpFilterString { get; set; }
        [Option("delete", Required = false, HelpText = "Delete file after watching.")]
        public bool DeleteAfterWatch { get; set; }
        [Option("random", Required = false, HelpText = "Random sort.", Default = false)]
        public bool Random { get; set; }
        [Option("random-continue", Required = false, HelpText = "Random sort, but started files first.")]
        public bool RandomContinue { get; set; }
        [Option("sort-by-name", Required = false, HelpText = "Sort by name.")]
        public bool SortByName { get; set; }
        [Option("extensions", Required = false, HelpText = "Extensions to use.", Default = "*.mkv;*.avi;*.mp4;*.webm;*.wmv;*.vob;*.ts;*.mpg;*.m4v;*.mp3;*.m4a;*.webm")]
        public string Extensions { get; set; }
        [Option("autonext", Required = false, HelpText = "Go next file after X seconds.", Default = 0)]
        public int AutoNext { get; set; }

        [Option("tomato", Required = false, HelpText = "Tomato timer in seconds.", Default = 0)]
        public int AutoClose { get; set; }

        [Option("volume", Required = false, HelpText = "Tomato timer in seconds.", Default = 100)]
        public int Volume { get; set; }

        public SortOrder SortOrder
        {
            get
            {
                if (Random)
                {
                    return SortOrder.Random;
                }
                if (RandomContinue)
                {
                    return SortOrder.RandomContinue;
                }
                if (SortByName)
                {
                    return SortOrder.ByName;
                }
                return SortOrder.Default;
            }
        }
    }
}
