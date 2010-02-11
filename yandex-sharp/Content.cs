//  
// Mono.Yandex.Fotki.Content.cs: Represents data that are being sent and received
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
using System.Xml;
using System.Text;

namespace Mono.Yandex.Fotki {
        internal interface IContent {
                string Name {
                        get;
                }

                string Type {
                        get;
                }

                Stream GetStream ();
        }

        internal class ImageContent: IContent {
                private string file_path;

                ImageContent (string filePath)
                {
                        if (!File.Exists (filePath))
                                return; //TODO exception
                        file_path = filePath;
                }

                public string Name {
                        get {
                                return Path.GetFileName (file_path);
                        }
                }

                public string Type {
                        get {
                                return WebHelper.GetMimeType (file_path);
                        }
                }

                public Stream GetStream () 
                {
                        using (FileStream fs = File.Open (file_path, FileMode.Open)) {
                                return fs;
                        }
                }
        }

        internal class AtomContent: IContent {
                private string type = "application/atom+xml; charset=utf-8; type=entry";
                private string content;
                
                AtomContent (string content)
                {
                        this.content = content;
                }

                public string Name {
                        get { return null; }
                }

                public string Type {
                        get { return type; }
                }

                public Stream GetStream ()
                {
                        using (MemoryStream ms = new MemoryStream (Encoding.UTF8.GetBytes (content))) {
                                return ms;
                        }
                }
        }
}
