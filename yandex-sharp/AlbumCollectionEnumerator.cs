//
// Mono.Yandex.Fotki.AlbumCollectionEnumerator.cs
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
                class AlbumCollectionEnumerator : IEnumerator<Album> {
                        private FeedEnumerator feed_enumerator;
                        private Album current_album;
                        private FotkiService fotki;

                        public AlbumCollectionEnumerator (string albumCollectionUrl, FotkiService fotkiService)
                        {
                                fotki = fotkiService;
                                feed_enumerator = new FeedEnumerator (albumCollectionUrl, fotki.Request);
                        }

                        public bool MoveNext ()
                        {
                                bool res = feed_enumerator.MoveNext ();
                                if (res == true)
                                        current_album = new Album (fotki, feed_enumerator.Current);
                                else
                                        current_album = null;

                                return res;
                        }

                        public void Reset ()
                        {
                                feed_enumerator.Reset ();
                                current_album = null;
                        }

                        public void Dispose ()
                        {
                        }

                        public Album Current {
                                get {
                                        if (current_album == null)
                                                throw new InvalidOperationException ();
                                        return current_album;
                                }
                        }

                        object IEnumerator.Current {
                                get {
                                        return Current;
                                }
                        }
                }
}

