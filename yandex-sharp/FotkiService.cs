//  
// Mono.Yandex.Fotki.FotkiService.cs: Main class for access to Yandex.Fotki
//
// Author:
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
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Net;

namespace Mono.Yandex.Fotki{
	public class FotkiService{
		private string user;
		private PhotoCollection photos;
		//private AlbumCollection albums;
		private string link_photos;
		private string link_albums;
                private string link_service_document = 
                        "http://api-fotki.yandex.ru/api/users/";
                private RequestManager request_manager;

		public FotkiService (string user)
		{
                        link_service_document += user + "/";

                        request_manager = new RequestManager ();
                        try {
                                string service_document = 
                                request_manager.GetString (link_service_document);
                                ParseServiceDocument (service_document);
                        } catch (WebException exception) {
                                HttpWebResponse response = (HttpWebResponse) exception.Response;
                                if (response != null) {
                                        HttpStatusCode status_code = response.StatusCode;

                                        string response_text;
                                        using (StreamReader reader = new StreamReader (response.GetResponseStream ())) {
                                                response_text = reader.ReadToEnd ();
                                        }

                                        if (status_code == HttpStatusCode.NotFound)
                                                throw new UserNotFoundException (response_text);
                                }

                                throw new ConnectionFailedException (exception.Message);
                        }
		}
		
		public FotkiService (string user, string password) : this(user)
		{
                        request_manager = new RequestManager (user, password);
		}
		
                public PhotoCollection GetPhotos ()
                {
                        if (photos == null) {
                                var photos_xml = request_manager.GetString (link_photos);
                                photos = new PhotoCollection (this, photos_xml);
                        }
                        return photos;
                }
                
                //public AlbumCollection GetAlbums ()
                //{
                //        if (albums == null) {
                //                var albums_xml = request_manager.GetString (link_albums);
                //                albums  = new AlbumCollection (albums_xml);
                //        }
                //        return albums;
                //}

                private void ParseServiceDocument (string serviceDocument)
                {
                        var document = new XmlDocument ();
                        document.LoadXml (serviceDocument);
                        var nav = document.CreateNavigator ();
			var mr = new XmlNamespaceManager (nav.NameTable);
			mr.AddNamespace ("app","http://www.w3.org/2007/app");
			mr.AddNamespace ("f","yandex:fotki");
			mr.AddNamespace ("atom","http://www.w3.org/2005/Atom");
			var iterator = nav.Select ("//app:collection/@href", mr);
                        iterator.MoveNext ();
			link_albums = iterator.Current.ToString ();
			iterator.MoveNext ();
			link_photos = iterator.Current.ToString ();
                }

                internal RequestManager Request {
                        get {
                                return request_manager;
                        }
                }
	}
}
