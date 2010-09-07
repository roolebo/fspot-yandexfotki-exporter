//
// Mono.Yandex.Fotki.Album.cs: represents album and ways of interaction with its content
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
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace Mono.Yandex.Fotki {
	public class Album : IEnumerable<Photo> {
                private string real_id;
                private string link_self;
                private string link_edit;
                private string link_photos;
                private string link_web;
                private string password_to_set;
                private FotkiService fotki;

		public uint Id { get; private set; }
		public string Title { get; set; }
		public string Author { get; private set; }
		public string Summary { get; set; }
		public uint Count { get; private set; }
                public DateTime Published { get; private set; }
                public DateTime Updated { get; private set; }
                public DateTime Edited { get; private set; }
                public bool IsProtected { get; private set; }

		internal Album (FotkiService fotki, string xml)
		{
                        this.fotki = fotki;
			InitAlbum (xml);
		}
                public Album ()
                {
                }

                public Photo Add (Photo photo)
                {
                        MultipartData data = MultipartDataHelper.BuildMultipartData (photo);

                        string response = fotki.Request.PostMultipart (link_photos, data);
                        return fotki.GetPhotos ().GetPhoto (
                                        uint.Parse ((HttpUtility.ParseQueryString (
                                                                response)) ["image_id"]));
                }

                public void AddAsync (Photo photo)
                {
                        UploadQueue.Instance (fotki).Add (photo);
                }

		public void Delete ()
		{
                        fotki.Request.Delete (link_edit);
		}

		public void Update ()
		{
                        string response;
			response = fotki.Request.PutAtomEntry (link_edit, BuildXmlForUpdate ());
                        InitAlbum (response);
		}

                public void SetPassword (string password)
                {
                        password_to_set = password;
                }

		public IEnumerator<Photo> GetEnumerator ()
		{
			return new PhotoCollectionEnumerator (link_photos, fotki);
		}

                IEnumerator IEnumerable.GetEnumerator ()
                {
                        return GetEnumerator ();
                }

                private string BuildXmlForUpdate ()
                {
                        XmlWriterSettings settings = new XmlWriterSettings ();
                        settings.Indent = true;
                        settings.OmitXmlDeclaration = true;
                        StringBuilder sb = new StringBuilder ();

                        XmlWriter writer = XmlWriter.Create (sb, settings);
                        writer.WriteStartElement ("entry");
                        writer.WriteAttributeString ("xmlns", "http://www.w3.org/2005/Atom");
                        writer.WriteAttributeString ("xmlns:app", "http://www.w3.org/2007/app");
                        writer.WriteAttributeString ("xmlns:f", "yandex:fotki");

                        writer.WriteElementString ("id", real_id);

                        writer.WriteStartElement ("author");
                        writer.WriteElementString ("name", Author);
                        writer.WriteEndElement ();

                        writer.WriteElementString ("title", Title);

                        writer.WriteElementString ("summary", Summary);

                        writer.WriteStartElement ("link");
                        writer.WriteAttributeString ("href", link_self);
                        writer.WriteAttributeString ("rel", "self");
                        writer.WriteEndElement ();

                        writer.WriteStartElement ("link");
                        writer.WriteAttributeString ("href", link_edit);
                        writer.WriteAttributeString ("rel", "edit");
                        writer.WriteEndElement ();

                        writer.WriteStartElement ("link");
                        writer.WriteAttributeString ("href", link_photos);
                        writer.WriteAttributeString ("rel", "photos");
                        writer.WriteEndElement ();

                        writer.WriteStartElement ("link");
                        writer.WriteAttributeString ("href", link_web);
                        writer.WriteAttributeString ("rel", "alternate");
                        writer.WriteEndElement ();

                        writer.WriteElementString ("published",
                                        DateTimeHelper.ConvertDateTimeToRfc3339 (Published));
                        writer.WriteElementString ("app:edited",
                                        DateTimeHelper.ConvertDateTimeToRfc3339 (Edited));
                        writer.WriteElementString ("updated",
                                        DateTimeHelper.ConvertDateTimeToRfc3339 (Updated));

                        writer.WriteStartElement ("f:protected");
                        writer.WriteAttributeString ("value", IsProtected.ToString ().ToLower ());
                        writer.WriteEndElement ();

                        writer.WriteStartElement ("f:image-count");
                        writer.WriteAttributeString ("value", Count.ToString ());
                        writer.WriteEndElement ();

                        if (!String.IsNullOrEmpty (password_to_set))
                                writer.WriteElementString ("f:password", password_to_set);

                        writer.WriteEndElement ();

                        writer.Close ();

                        return sb.ToString ();
                }

                internal string BuildXmlForFirstUpload ()
                {
                        XmlWriterSettings settings = new XmlWriterSettings ();
                        settings.Indent = true;
                        settings.OmitXmlDeclaration = true;
                        StringBuilder sb = new StringBuilder ();

                        XmlWriter writer = XmlWriter.Create (sb, settings);
                        writer.WriteStartElement ("entry");
                        writer.WriteAttributeString ("xmlns", "http://www.w3.org/2005/Atom");
                        writer.WriteAttributeString ("xmlns:app", "http://www.w3.org/2007/app");
                        writer.WriteAttributeString ("xmlns:f", "yandex:fotki");

                        writer.WriteElementString ("title", Title);

                        writer.WriteElementString ("summary", Summary);

                        if (!String.IsNullOrEmpty (password_to_set))
                                writer.WriteElementString ("f:password", password_to_set);

                        writer.WriteEndElement ();

                        writer.Close ();

                        return sb.ToString ();
                }

		private void InitAlbum (string xml)
		{
                        var document = new XmlDocument ();
                        document.LoadXml (xml);
			var nav = document.CreateNavigator ();
			XmlNamespaceManager mr = new XmlNamespaceManager (nav.NameTable);
			mr.AddNamespace ("app","http://www.w3.org/2007/app");
			mr.AddNamespace ("f","yandex:fotki");
			mr.AddNamespace ("atom","http://www.w3.org/2005/Atom");

                        real_id = (string) nav.Evaluate ("string(//atom:id)", mr);
			Id = uint.Parse ((string) nav.Evaluate ("substring-after(//atom:id,':album:')", mr));

			Title = (string) nav.Evaluate ("string(//atom:title)", mr);
			Author = (string) nav.Evaluate ("string(//atom:author/atom:name)", mr);
			Summary = (string) nav.Evaluate ("string(//atom:summary)", mr);
                        IsProtected = bool.Parse ((string) nav.Evaluate ("string(//f:protected/@value)", mr));
                        Count = uint.Parse ((string) nav.Evaluate ("string(//f:image-count/@value)",mr));

                        string temp_date;
                        temp_date = (string) nav.Evaluate ("string(//app:edited)", mr);
                        Edited = DateTimeHelper.ConvertRfc3339ToDateTime (temp_date);
                        temp_date = (string) nav.Evaluate ("string(//atom:updated)", mr);
                        Updated = DateTimeHelper.ConvertRfc3339ToDateTime (temp_date);
                        temp_date = (string) nav.Evaluate ("string(//atom:published)", mr);
                        Published = DateTimeHelper.ConvertRfc3339ToDateTime (temp_date);

                        link_self = (string) nav.Evaluate ("string(//atom:link[@rel='self']/@href)", mr);
                        link_edit = (string) nav.Evaluate ("string(//atom:link[@rel='edit']/@href)", mr);
                        link_web = (string) nav.Evaluate ("string(//atom:link[@rel='alternate']/@href)", mr);
                        link_photos = (string) nav.Evaluate ("string(//atom:link[@rel='photos']/@href)", mr);
                        password_to_set = null;
		}
	}
}
