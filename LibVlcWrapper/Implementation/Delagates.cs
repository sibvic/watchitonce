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
using System.Runtime.InteropServices;
using LibVlcWrapper;

namespace Declarations
{
   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   delegate void VlcEventHandlerDelegate(ref libvlc_event_t libvlc_event, IntPtr userData);

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   unsafe delegate void* LockEventHandler(void* opaque, void** plane);

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   unsafe delegate void UnlockEventHandler(void* opaque, void* picture, void** plane);

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   unsafe delegate void DisplayEventHandler(void* opaque, void* picture);

   [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
   unsafe delegate void* CallbackEventHandler(void* data);
}
