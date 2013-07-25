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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;

namespace Declarations
{
   /// <summary>
   /// Specifies the parameters of the bitmap.
   /// </summary>
   public class BitmapFormat
   {
      int m_width;
      int m_height;
      int m_bpp;
      ChromaType m_chroma;
      PixelFormat m_pixelFormat;

      /// <summary>
      /// Initializes new instance of BitmapFormat class
      /// </summary>
      /// <param name="width">The width of the bitmap in pixels</param>
      /// <param name="height">The height of the bitmap in pixels</param>
      /// <param name="chroma">Chroma type of the bitmap</param>
      public BitmapFormat(int width, int height, ChromaType chroma)
      {
         m_width = width;
         m_height = height;
         m_chroma = chroma;

         Init();
         
         Pitch = m_width * m_bpp;
         ImageSize = Pitch * height;
         Chroma = m_chroma.ToString();
      }

      private void Init()
      {
         switch (m_chroma)
         {
            case ChromaType.RV15:
               m_pixelFormat = PixelFormat.Format16bppRgb555;
               m_bpp = 2;
               break;

            case ChromaType.RV16:
               m_pixelFormat = PixelFormat.Format16bppRgb565;
               m_bpp = 2;
               break;

            case ChromaType.RV24:
               m_pixelFormat = PixelFormat.Format24bppRgb;
               m_bpp = 3;
               break;

            case ChromaType.RV32:
               m_pixelFormat = PixelFormat.Format32bppRgb;
               m_bpp = 4;
               break;

            case ChromaType.RGBA:
               m_pixelFormat = PixelFormat.Format32bppArgb;
               m_bpp = 4;
               break;
         }
      }

      /// <summary>
      /// Gets the size in bytes of the scan line 
      /// </summary>
      public int Pitch { get; private set; }

      /// <summary>
      /// Gets the size of the image
      /// </summary>
      public int ImageSize { get; private set; }

      /// <summary>
      /// Gets the chroma type string
      /// </summary>
      public string Chroma { get; private set; }

      /// <summary>
      /// Gets the pixel format of the bitmap
      /// </summary>
      public PixelFormat PixelFormat
      {
         get
         {
            return m_pixelFormat;
         }
      }

      /// <summary>
      /// Gets the width of the bitmap
      /// </summary>
      public int Width
      {
         get
         {
            return m_width;
         }
      }

      /// <summary>
      /// Gets the height of the bitmap
      /// </summary>
      public int Height
      {
         get
         {
            return m_height;
         }
      }

      /// <summary>
      /// Gets number of bytes used for a pixel according to ChromaType
      /// </summary>
      public int BytesPerBixel
      {
         get
         {
            return m_bpp;
         }
      }
   }
}
