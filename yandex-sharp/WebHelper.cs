//  
// Mono.Yandex.Fotki.WebHelper.cs: Contains static methods that help to do something concerned web
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
using System.Net;

namespace Mono.Yandex.Fotki {
        class WebHelper {
                public static string GetMimeType (string filePath) {
                        string extension = Path.GetExtension (filePath).ToLower ();

                        if (extension == ".jpg" || extension == ".jpeg")
                                return "image/jpeg";
                        else if (extension == ".png")
                                return "image/png";
                        else if (extension == ".bmp")
                                return "image/bmp";
                        else if (extension == ".gif")
                                return "image/gif";
                        else
                                return "application/octet-stream";
                }

                public static string GetResponseString (HttpWebRequest request)
                {
                        using (HttpWebResponse response = (HttpWebResponse) request.GetResponse ()) {
                                Stream response_stream = response.GetResponseStream ();
                                StreamReader reader = new StreamReader (response_stream);
                                return reader.ReadToEnd ();
                        }
                }

        }
}
