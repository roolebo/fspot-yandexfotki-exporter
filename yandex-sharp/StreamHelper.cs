//  
// Mono.Yandex.Fotki.StreamHelper.cs: Helps to perform some operations with Stream
//
// Author:
//    Roman Bolshakov
//
// Copyright (C) 2010 Roman Bolshakov
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
using System;
using System.IO;

namespace Mono.Yandex.Fotki {
        class StreamHelper {
                public static void CopyStream (Stream input, Stream output)
                {
                        byte[] buffer = ReadFully (input);

                        output.Write (buffer, 0, buffer.Length);
                }

                public static byte[] ReadFully (Stream input)
                {
                        byte[] buffer = new byte[1024];
                        using (MemoryStream output = new MemoryStream ()) {
                                int read;

                                while((read = input.Read(buffer, 0, buffer.Length)) > 0) 
                                        output.Write(buffer, 0, read);
                                return output.ToArray ();
                        }
                }
        }
}
