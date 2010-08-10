//  
// Mono.Yandex.Fotki.Photo.cs: represents photo and ways of interaction with its content
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
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Text;

namespace Mono.Yandex.Fotki{

	public class Photo {
                private bool adult_content;
                private string link_self;
                private string link_edit;
                private string link_web;
                private string link_edit_media;
                private string link_album;
                private string link_content;
                private FotkiService fotki;

                internal string Filepath { get; set; }

                public enum Access {Public, Friends, Private}
                public enum Size {Original, XL, L, M, S, XS, XXS, XXXS}

		public uint Id { get; private set; }
		public string Title { get; set; }
		public string Author { get; private set; }
		public bool DisableComments { get; set; }
		public bool HideOriginal { get; set; }
                public bool IsReadOnly { get; private set; }
                public DateTime Created { get; private set; }
                public DateTime Published { get; private set; }
                public DateTime Updated { get; private set; }
                public DateTime Edited { get; private set; }
                public Access AccessLevel { get; set; }
		public bool AdultContent {
                        get {
                                return adult_content;
                        }
                        set {
                                if (adult_content != true)
                                        adult_content = value;
                        }
                }


		public Photo (string filepath) 
                {
                        Filepath = filepath;
                        //TODO check whether the file exist
                }
		
		internal Photo (FotkiService fotki, string xml)
		{
                        this.fotki = fotki;
			InitPhoto (xml);
		}
		
		public void Delete ()
		{
                        fotki.Request.Delete (link_self);
                        //TODO something after deleting
		}
		
		public byte[] Download (Size photoSize)
		{
                        string suffix = "XL";
                        switch (photoSize) {
                                case Size.Original:
                                        suffix = "orig";
                                        break;
                                case Size.XL:
                                        suffix = "XL";
                                        break;
                                case Size.L:
                                        suffix = "L";
                                        break;
                                case Size.M:
                                        suffix = "M";
                                        break;
                                case Size.S:
                                        suffix = "S";
                                        break;
                                case Size.XS:
                                        suffix = "XS";
                                        break;
                                case Size.XXS:
                                        suffix = "XXS";
                                        break;
                                case Size.XXXS:
                                        suffix = "XXXS";
                                        break;
                        }

                        int last_underscore_position = link_content.LastIndexOf ('_');
                        string link = link_content.Substring(0, last_underscore_position) + suffix;
			return fotki.Request.GetBinary (link);
		}
		
		/*public void Update ()
		{
			RequestManager.Edit (this);
		}
		*/

                //internal string FormXmlDocument ()
                //{

                //}
		
		private void InitPhoto (string xml)
                {
                        var document = new XmlDocument ();
                        document.LoadXml (xml);
			var nav = document.CreateNavigator ();
			XmlNamespaceManager mr = new XmlNamespaceManager (nav.NameTable);
			mr.AddNamespace ("app","http://www.w3.org/2007/app");
			mr.AddNamespace ("f","yandex:fotki");
			mr.AddNamespace ("atom","http://www.w3.org/2005/Atom");
			
			Id = UInt32.Parse((string) nav.Evaluate ("substring-after(/atom:entry/atom:id,':photo:')", mr));
			
			Title = (string) nav.Evaluate ("string(/atom:entry/atom:title)", mr);
			Author = (string) nav.Evaluate ("string(/atom:entry/atom:author/atom:name)", mr);
			AdultContent = bool.Parse ((string) nav.Evaluate ("string(/atom:entry/f:xxx/@value)", mr));
			HideOriginal = bool.Parse ((string) nav.Evaluate ("string(/atom:entry/f:hide_original/@value)", mr));
			DisableComments = bool.Parse ((string) nav.Evaluate ("string(/atom:entry/f:disable_comments/@value)", mr));
                        var temp_date = (string) nav.Evaluate ("string(/atom:entry/f:created", mr);
                        Created = XmlConvert.ToDateTime (temp_date, "yyyy-MM-ddTHH:mm:ss");
                        temp_date = (string) nav.Evaluate ("string(/atom:entry/app:edited", mr);
                        Edited = XmlConvert.ToDateTime (temp_date, "yyyy-MM-ddTHH:mm:ss");
                        temp_date = (string) nav.Evaluate ("string(/atom:entry/atom:updated", mr);
                        Updated = XmlConvert.ToDateTime (temp_date, "yyyy-MM-ddTHH:mm:ss");
                        temp_date = (string) nav.Evaluate ("string(/atom:entry/atom:published", mr);
                        Published = XmlConvert.ToDateTime (temp_date, "yyyy-MM-ddTHH:mm:ss");

                        string access = (string) nav.Evaluate ("string(/atom:entry/f:access)", mr);
                        if (access == "public")
                                AccessLevel = Access.Public;
                        else if (access == "friends")
                                AccessLevel = Access.Friends;
                        else
                                AccessLevel = Access.Private;

                        link_self = (string) nav.Evaluate ("string(/atom:entry/atom:link[@rel=self]/@href", mr);
                        link_edit = (string) nav.Evaluate ("string(/atom:entry/atom:link[@rel=edit]/@href", mr);
                        link_web = (string) nav.Evaluate ("string(/atom:entry/atom:link[@rel=alternate]/@href", mr);
                        link_album = (string) nav.Evaluate ("string(/atom:entry/atom:link[@rel=album]/@href", mr);
                        link_edit_media = (string) nav.Evaluate ("string(/atom:entry/atom:link[@rel=edit-media]/@href", mr);
                        link_content = (string) nav.Evaluate ("string(/atom:entry/atom:content/@src", mr);
		}
	}
}
