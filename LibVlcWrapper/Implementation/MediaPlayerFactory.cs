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
using Declarations;
using Declarations.Media;
using Declarations.Players;
using Implementation.Exceptions;
using Implementation.Media;
using Implementation.Players;
using LibVlcWrapper;

namespace Implementation
{
   /// <summary>
   /// Entry point for the nVLC library.
   /// </summary>
   public class MediaPlayerFactory : DisposableBase, IMediaPlayerFactory, IReferenceCount, INativePointer
   {
      IntPtr m_hMediaLib = IntPtr.Zero;
      Log m_log;
     
      /// <summary>
      /// Initializes media library with default arguments
      /// </summary>
      public MediaPlayerFactory(string playerPath)
      {
         string[] args = new string[] 
         {
            "-I", 
            "dumy",  
		      "--ignore-config", 
            "--no-osd",
            "--disable-screensaver",
		      "--plugin-path=" + System.IO.Path.Combine(playerPath, "plugins")
         };

         Initialize(args);
      }

      /// <summary>
      /// Initializes media library with user defined arguments
      /// </summary>
      /// <param name="args">Collection of arguments passed to libVLC library</param>
      public MediaPlayerFactory(string[] args)
      {
         Initialize(args);
      }

      private void Initialize(string[] args)
      {
         try
         {
            m_hMediaLib = LibVlcMethods.libvlc_new(args.Length, args);
         }
         catch (DllNotFoundException ex)
         {
            throw new LibVlcNotFoundException(ex);
         }

         if (m_hMediaLib == IntPtr.Zero)
         {
            throw new LibVlcInitException();
         }

         m_log = new Log(m_hMediaLib, new NLogger());
         m_log.Enabled = true;
      }

      /// <summary>
      /// Creates new instance of player.
      /// </summary>
      /// <typeparam name="T">Type of the player to create</typeparam>
      /// <returns>Newly created player</returns>
      public T CreatePlayer<T>() where T : IPlayer
      {
         return ObjectFactory.Build<T>(m_hMediaLib);
      }

      /// <summary>
      /// Creates new instance of media list player
      /// </summary>
      /// <typeparam name="T">Type of media list player</typeparam>
      /// <param name="mediaList">Media list</param>
      /// <returns>Newly created media list player</returns>
      public T CreateMediaListPlayer<T>(IMediaList mediaList) where T : IMediaListPlayer
      {
         return ObjectFactory.Build<T>(m_hMediaLib, mediaList);
      }

      /// <summary>
      /// Creates new instance of media.
      /// </summary>
      /// <typeparam name="T">Type of media to create</typeparam>
      /// <param name="input">The media input string</param>
      /// <param name="options">Optional media options</param>
      /// <returns>Newly created media</returns>
      public T CreateMedia<T>(string input, params string[] options) where T : IMedia
      {
          if (input.StartsWith("//"))
              input = "file:" + input;
         T media = ObjectFactory.Build<T>(m_hMediaLib);
         media.Input = input;
         media.AddOptions(options);

         return media;
      }

      /// <summary>
      /// Creates new instance of media list.
      /// </summary>
      /// <typeparam name="T">Type of media list</typeparam>
      /// <param name="mediaItems">Collection of media inputs</param>
      /// <returns>Newly created media list</returns>
      public T CreateMediaList<T>(IEnumerable<string> mediaItems) where T : IMediaList
      {
         T mediaList = ObjectFactory.Build<T>(m_hMediaLib);
         foreach (var file in mediaItems)
         {
            mediaList.Add(this.CreateMedia<IMedia>(file));
         }

         return mediaList;
      }

      /// <summary>
      /// Gets the libVLC version.
      /// </summary>
      public string Version
      {
         get
         {
            return LibVlcMethods.libvlc_get_version();
         }
      }

      protected override void Dispose(bool disposing)
      {
         Release();
      }

      private static class ObjectFactory
      {
         static Dictionary<Type, Type> objectMap = new Dictionary<Type, Type>();
      
         static ObjectFactory()
         {
            objectMap.Add(typeof(IMedia), typeof(BasicMedia));
            objectMap.Add(typeof(IMediaFromFile), typeof(MediaFromFile));
            objectMap.Add(typeof(IVideoInputMedia), typeof(VideoInputMedia));
            objectMap.Add(typeof(IScreenCaptureMedia), typeof(ScreenCaptureMedia));
            objectMap.Add(typeof(IPlayer), typeof(BasicPlayer));
            objectMap.Add(typeof(IAudioPlayer), typeof(AudioPlayer));
            objectMap.Add(typeof(IVideoPlayer), typeof(VideoPlayer));
            objectMap.Add(typeof(IDiskPlayer), typeof(DiskPlayer));
            objectMap.Add(typeof(IMediaList), typeof(MediaList));
            objectMap.Add(typeof(IMediaListPlayer), typeof(MediaListPlayer));
         }

         public static T Build<T>(params object[] args)
         {
            if(objectMap.ContainsKey(typeof(T)))
            {
               return (T)Activator.CreateInstance(objectMap[typeof(T)], args);
            }

            throw new ArgumentException("Unregistered type", typeof(T).FullName);
         }
      }

      #region IReferenceCount Members

      public void AddRef()
      {
         LibVlcMethods.libvlc_retain(m_hMediaLib);
      }

      public void Release()
      {
         try
         {
            LibVlcMethods.libvlc_release(m_hMediaLib);
         }
         catch (AccessViolationException)
         { }
      }

      #endregion

      #region INativePointer Members

      public IntPtr Pointer
      {
         get 
         {
            return m_hMediaLib;
         }
      }

      #endregion
   }
}
