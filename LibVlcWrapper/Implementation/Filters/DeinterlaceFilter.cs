//    nVLC
//    
//    Author:  Roman Ginzburg
//
//    nVLC is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    nVLC is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU General Public License for more details.
//     
// ========================================================================

using System;
using Declarations;
using Declarations.Filters;
using LibVlcWrapper;

namespace Implementation.Filters
{
   internal class DeinterlaceFilter : IDeinterlaceFilter
   {
      IntPtr m_hMediaPlayer;
      bool m_enabled = false;

      public DeinterlaceFilter(IntPtr hMediaPlayer)
      {
         m_hMediaPlayer = hMediaPlayer;
      }

      #region IDeinterlaceFilter Members

      public bool Enabled
      {
         get
         {
            return m_enabled;
         }
         set
         {
            LibVlcMethods.libvlc_video_set_deinterlace(m_hMediaPlayer, Mode.ToString().ToUtf8());
            m_enabled = true;
         }
      }

      public DeinterlaceMode Mode { get; set; }

      #endregion
   }
}
