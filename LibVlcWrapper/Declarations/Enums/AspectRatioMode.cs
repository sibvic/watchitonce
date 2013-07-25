//
//  Monkey Manager (Based on nVLC (by Roman Ginzburg))
//  Copyright (C) 2010 Victor Tereschenko (aka sibvic)
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
using System.ComponentModel;

namespace Declarations
{
   /// <summary>
   /// Available video aspect ratio modes
   /// </summary>
   public enum AspectRatioMode
   {
      /// <summary>
      /// Default aspect ratio of the frame
      /// </summary>
      [Description("Default")]
      Default = 0,

      /// <summary>
      /// 1:1
      /// </summary>
      [Description("1:1")]
      Mode1_1,

      /// <summary>
      /// 4:3
      /// </summary>
      [Description("4:3")]
      Mode4_3,

      /// <summary>
      /// 16:0
      /// </summary>
      [Description("16:9")]
      Mode16_9,

      /// <summary>
      /// 16:10
      /// </summary>
      [Description("16:10")]
      Mode16_10,

      /// <summary>
      /// 2.21:1
      /// </summary>
      [Description("2.21:1")]
      Mode2_21_1,

      /// <summary>
      /// 2.35:1
      /// </summary>
      [Description("2.35:1")]
      Mode2_35_1,

      /// <summary>
      /// 2:39:1
      /// </summary>
      [Description("2:39:1")]
      Mode2_39_1,

      /// <summary>
      /// 5:4
      /// </summary>
      [Description("5:4")]
      Mode5_4,
   }
}