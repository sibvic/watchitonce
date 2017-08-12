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


namespace WatchItOnce
{
    public class MediaFile
    {
        public MediaFile(string path, long position)
        {
            Path = path;
            PositionSeconds = position;
        }

        public string Path { get; private set; }
        public long PositionSeconds { get; private set; }
    }
}
