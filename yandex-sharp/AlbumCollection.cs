//
// Mono.Yandex.Fotki.AlbumCollection.cs
//
// Authors:
//    Maxim Kolchin
//    Roman Bolshakov
//
// Copyright (C) 2010 Maxim Kolchin
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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Yandex.Fotki {
	public class AlbumCollection : IEnumerable<Album> {
		private string link_self;
                //private string link_next_page;
                private FotkiService fotki;

                public DateTime Updated { get; private set; }

                internal AlbumCollection (FotkiService fotki, string xml)
                {
                        this.fotki = fotki;
                        ParseXml (xml);
                }

		public Album Add (Album album)
		{
                        if (String.IsNullOrEmpty (album.Title))
                                throw new InvalidOperationException (
                                                "Album's Title cannot be null or empty.");

                        string xml = album.BuildXmlForFirstUpload ();
                        string response = fotki.Request.PostAtomEntry (link_self, xml);

                        string updated_collection = fotki.Request.GetString (link_self);
                        ParseXml (updated_collection);

                        return new Album (fotki, response);
		}

		public IEnumerator<Album> GetEnumerator ()
		{
			return new AlbumCollectionEnumerator (link_self, fotki);
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		private void ParseXml (string xml)
		{
                        XmlDocument document = new XmlDocument ();
                        document.LoadXml (xml);
			XPathNavigator nav = document.CreateNavigator ();
			XmlNamespaceManager mr = new XmlNamespaceManager (nav.NameTable);
			mr.AddNamespace ("app","http://www.w3.org/2007/app");
			mr.AddNamespace ("f","yandex:fotki");
			mr.AddNamespace ("atom","http://www.w3.org/2005/Atom");

			link_self = (string) nav.Evaluate ("string(/atom:feed/atom:link[@rel='self']/@href)", mr);
			//link_next_page = (string) nav.Evaluate ("string(//atom:link[@rel='next']/@href)", mr);

                        string updated = (string) nav.Evaluate ("string(/atom:feed/atom:updated)", mr);
                        Updated = DateTimeHelper.ConvertRfc3339ToDateTime (updated);

                }
        }
}
