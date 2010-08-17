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

namespace Mono.Yandex.Fotki{
	
	public class PhotoCollection : IEnumerable<Photo> {
		private Dictionary<uint, Photo> photos;
                private FotkiService fotki;
                private string xml;
                private string link_self;
                private string link_next_page;
                private string post_uri = "http://api-fotki.yandex.ru/post/";

		internal PhotoCollection (FotkiService fotki, string xml)
		{
                        this.fotki = fotki;
			photos = new Dictionary<uint, Photo> ();
                        this.xml = xml;
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

		public IEnumerator<Photo> GetEnumerator ()
		{
			return new PhotoCollectionEnumerator (this);
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


			//XPathNodeIterator iterator = nav.Select ("/atom:feed/atom:entry",mr);
			//while(iterator.MoveNext ()){
			//	photos.Add (new Photo 
			//	            (new XPathDocument 
			//	             (iterator.Current.ReadSubtree())));
			//}
		}

                class PhotoCollectionEnumerator : IEnumerator<Photo> {
                        private PhotoCollection photo_collection;
                        private XmlDocument document;
                        private XmlNamespaceManager manager;
                        private XPathNavigator navigator;
                        private XPathNodeIterator iterator;
                        private XPathExpression entry_expression;
                        private XPathExpression next_page_expression;
                        private Photo current;
                        private string link_next_page;

                        public PhotoCollectionEnumerator (PhotoCollection photo_collection)
                        {
                                this.photo_collection = photo_collection;
                                document = new XmlDocument ();

                                entry_expression = XPathExpression.Compile (
                                                "/atom:feed/atom:entry");
                                next_page_expression = XPathExpression.Compile (
                                                "string(/atom:feed/atom:link[@rel='next']/@href)");

                                LoadXml (photo_collection.xml);
                                link_next_page = photo_collection.link_next_page;
                        }

                        public bool MoveNext ()
                        {
                                bool res = iterator.MoveNext ();
                                if (res == false && !String.IsNullOrEmpty (link_next_page)) {
                                        string next_page = photo_collection.fotki.Request.GetString (link_next_page);
                                        LoadXml (next_page);
                                        link_next_page = GetNextPageUrl ();
                                        //move to entry on newly loaded page
                                        res = iterator.MoveNext ();
                                }

                                if (res == true)
                                        current = new Photo (photo_collection.fotki,
                                                        iterator.Current.OuterXml);
                                else
                                        current = null;

                                return res;
                        }

                        public void Reset ()
                        {
                                LoadXml (photo_collection.xml);
                                link_next_page = photo_collection.link_next_page;
                                current = null;
                        }

                        public void Dispose ()
                        {
                        }

                        public Photo Current {
                                get {
                                        if (current == null)
                                                throw new InvalidOperationException ();
                                        return current;
                                }
                        }

                        object IEnumerator.Current {
                                get {
                                        return Current;
                                }
                        }

                        string GetNextPageUrl ()
                        {
                                return (string) navigator.Evaluate ("string(//atom:link[@rel='next']/@href)", manager);
                        }

                        void LoadXml (string xml)
                        {
                                document.LoadXml (xml);
                                navigator = document.CreateNavigator ();

                                manager = new XmlNamespaceManager (navigator.NameTable);
                                manager.AddNamespace ("app","http://www.w3.org/2007/app");
                                manager.AddNamespace ("f","yandex:fotki");
                                manager.AddNamespace ("atom","http://www.w3.org/2005/Atom");

                                entry_expression.SetContext (manager);
                                next_page_expression.SetContext (manager);

                                iterator = navigator.Select (entry_expression);
                        }
                }
	}
}
