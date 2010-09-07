//
// Mono.Yandex.Fotki.PhotoCollection.cs: access to collection of all photos
//
// Authors:
//    Maxim Kolchin, Roman Bolshakov
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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Web;

namespace Mono.Yandex.Fotki {
	public class PhotoCollection : IEnumerable<Photo> {
		private Dictionary<uint, Photo> photos;
                private FotkiService fotki;
                private string xml;
                private string link_self;
                private string link_next_page;
                internal const string post_uri = "http://api-fotki.yandex.ru/post/";
                private string photo_url_prefix;

                public DateTime Updated { get; private set; }

		internal PhotoCollection (FotkiService fotki, string xml)
		{
                        this.fotki = fotki;
			photos = new Dictionary<uint, Photo> ();
                        this.xml = xml;
			ParseXml (xml);
		}

                internal Photo GetPhoto (uint index)
                {
                        //TODO add exception
                        string link_to_photo = photo_url_prefix + index.ToString () + "/";
                        string xml = fotki.Request.GetString (link_to_photo);
                        Photo photo = new Photo (fotki, xml);

                        return photo;
                }

                public Photo Add (Photo photo)
                {
                        MultipartData data = MultipartDataHelper.BuildMultipartData (photo);

                        string response = fotki.Request.PostMultipart (post_uri, data);
                        return GetPhoto (uint.Parse ((HttpUtility.ParseQueryString
                                                (response)) ["image_id"]));
                }

                public void AddAsync (Photo photo)
                {
                        UploadQueue.Instance (fotki).Add (photo);
                }

                public void RemovePhoto (uint index)
                {
                        //TODO add exception
                        fotki.Request.Delete (link_self + index.ToString ());
                }

		public IEnumerator<Photo> GetEnumerator ()
		{
			return new PhotoCollectionEnumerator (link_self, fotki);
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
			link_next_page = (string) nav.Evaluate ("string(//atom:link[@rel='next']/@href)", mr);

                        string updated = (string) nav.Evaluate ("string(/atom:feed/atom:updated)", mr);
                        Updated = DateTimeHelper.ConvertRfc3339ToDateTime (updated);

                        photo_url_prefix = link_self.Substring (0, link_self.Length - 2) + "/";
		}

                private void UploadPhotosFromQueue ()
                {
                        Photo photo, uploaded_photo;

                        Monitor.Enter (photos_to_upload);
                        while (photos_to_upload.Count > 0) {
                                Monitor.Exit (photos_to_upload);

                                photo = photos_to_upload.Dequeue ();
                                MultipartData data = MultipartDataHelper.BuildMultipartData (photo);
                                string response = fotki.Request.PostMultipart (
                                                PhotoCollection.post_uri, data, true);
                                uint image_id = uint.Parse ((HttpUtility.ParseQueryString
                                                                (response)) ["image_id"]);
                                uploaded_photo = fotki.GetPhotos ().GetPhoto (image_id);
                                var args = new UploadPhotoCompletedEventArgs (
                                                uploaded_photo);
                                fotki.OnUploadPhotoCompleted (args);

                                Monitor.Enter (photos_to_upload);
                        }

                        Monitor.Exit (photos_to_upload);
                }
	}
}
