//
// Mono.Yandex.Fotki.PhotoCollectionEnumerator.cs
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
using System.Collections;
using System.Collections.Generic;

namespace Mono.Yandex.Fotki {
                class PhotoCollectionEnumerator : IEnumerator<Photo> {
                        private FeedEnumerator feed_enumerator;
                        private Photo current_photo;
                        private FotkiService fotki;

                        public PhotoCollectionEnumerator (string photoCollectionUrl, FotkiService fotkiService)
                        {
                                fotki = fotkiService;
                                feed_enumerator = new FeedEnumerator (photoCollectionUrl, fotki.Request);
                        }

                        public bool MoveNext ()
                        {
                                bool res = feed_enumerator.MoveNext ();
                                if (res == true)
                                        current_photo = new Photo (fotki, feed_enumerator.Current);
                                else
                                        current_photo = null;

                                return res;
                        }

                        public void Reset ()
                        {
                                feed_enumerator.Reset ();
                                current_photo = null;
                        }

                        public void Dispose ()
                        {
                        }

                        public Photo Current {
                                get {
                                        if (current_photo == null)
                                                throw new InvalidOperationException ();
                                        return current_photo;
                                }
                        }

                        object IEnumerator.Current {
                                get {
                                        return Current;
                                }
                        }
                }
}
