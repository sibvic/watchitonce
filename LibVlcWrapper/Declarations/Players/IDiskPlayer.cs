﻿//    nVLC
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Declarations.Players
{
   /// <summary>
   /// DVD, VCD and Audio CD playback
   /// </summary>
   public interface IDiskPlayer : IVideoPlayer
   {
      /// <summary>
      /// Gets the number of chapters in the movie.
      /// </summary>
      int ChapterCount { get; }

      /// <summary>
      /// Gets or sets a chapter.
      /// </summary>
      int Chapter { get; set; }

      /// <summary>
      /// Gets or sets the movie title
      /// </summary>
      int Title { get; set; }

      /// <summary>
      /// Gets the number of chapters for specified title.
      /// </summary>
      /// <param name="title">title</param>
      /// <returns>Number of chapters</returns>
      int GetChapterCountForTitle(int title);

      /// <summary>
      /// Sets the next chapter.
      /// </summary>
      void NextChapter();

      /// <summary>
      /// Sets the previos chapter.
      /// </summary>
      void PreviousChapter();

      /// <summary>
      /// Sets or gets the audio track.
      /// </summary>
      int AudioTrack { get; set; }

      /// <summary>
      /// Gets the number of audio tracks.
      /// </summary>
      int AudioTrackCount { get; }

      /// <summary>
      /// Gets or sets video subtitle
      /// </summary>
      int SubTytle { get; set; }

      /// <summary>
      /// Gets the number of video subtitles
      /// </summary>
      int SubTytleCount { get; }

      /// <summary>
      /// Get the number of video tracks.
      /// </summary>
      int VideoTrackCount { get; }

      /// <summary>
      /// Gets or sets video track.
      /// </summary>
      int VideoTrack { get; set; }
   }
}
