//  
// Mono.Yandex.Fotki.PhotoCollection.cs: access to collection of all photos
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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Web;

namespace Mono.Yandex.Fotki{
	
	public class PhotoCollection {
		private Dictionary<uint, Photo> photos;
                private FotkiService fotki;
                private string link_self;
                private string link_next_page;
                private string post_uri = "http://api-fotki.yandex.ru/post/";

		internal PhotoCollection (FotkiService fotki, string xml)
		{
                        this.fotki = fotki;
			photos = new Dictionary<uint, Photo> ();
			ParseXml (xml);
		}

                public Photo GetPhoto (uint index)
                {
                        if (!photos.ContainsKey (index)) {
                                //TODO add exception
                                string link_to_photo = link_self + index.ToString () + "/";
                                string xml = fotki.Request.GetString (link_to_photo);
                                Photo photo = new Photo (xml);
                                photos.Add (index, photo);
                        }

                        return photos[index];
                }
                public uint Add (Photo photo)
                {
                        MultipartData data = new MultipartData ();
                        data.Add (new MultipartData.Parameter ("image", photo.Filepath, MultipartData.Parameter.ParamType.File));
                        data.Add (new MultipartData.Parameter ("title", photo.Title, MultipartData.Parameter.ParamType.Field));
                        string access;
                        if (photo.AccessLevel == Access.Public)
                                access = "public";
                        else if (photo.AccessLevel == Access.Friends)
                                access = "friends";
                        else
                                access = "private";
                        data.Add (new MultipartData.Parameter ("access_type", access, MultipartData.Parameter.ParamType.Field));
                        string disable_comments;
                        if (photo.DisableComments == true)
                                disable_comments = "true";
                        else
                                disable_comments = "false";
                        data.Add (new MultipartData.Parameter ("disable_comments", disable_comments, MultipartData.Parameter.ParamType.Field));
                        string hide_original;
                        if (photo.HideOriginal == true)
                                hide_original = "true";
                        else
                                hide_original = "false";
                        data.Add (new MultipartData.Parameter ("hide_orignal", hide_original, MultipartData.Parameter.ParamType.Field));
                        string xxx;
                        if (photo.AdultContent == true)
                                xxx = "true";
                        else
                                xxx = "false";
                        data.Add (new MultipartData.Parameter ("xxx", xxx, MultipartData.Parameter.ParamType.Field));

                        string response = fotki.Request.PostMultipart (post_uri, data);
                        return uint.Parse ((HttpUtility.ParseQueryString 
                                                (response)) ["image_id"]);
                }
                public void RemovePhoto (uint index)
                {
                        //TODO add exception
                        fotki.Request.Delete (link_self + index.ToString ());
                }
		//public IEnumerator GetEnumerator ()
		//{
		//	return photos.GetEnumerator();
		//}
		
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
			
			//XPathNodeIterator iterator = nav.Select ("/atom:feed/atom:entry",mr);
			//while(iterator.MoveNext ()){
			//	photos.Add (new Photo 
			//	            (new XPathDocument 
			//	             (iterator.Current.ReadSubtree())));
			//}
		}
	}
}
