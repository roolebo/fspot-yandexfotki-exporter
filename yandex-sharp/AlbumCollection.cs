using System;
using System.IO;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Yandex.Fotki {

	public class AlbumCollection{
		public string link;
		private ArrayList albums;
		
		public AlbumCollection (XPathDocument xml)
		{
			this.albums = new ArrayList ();
			ParseXml (xml);
		}
		
		public void Add (Album album)
		{
			//RequestMananger.Add (album);
		}
		
		public IEnumerator GetEnumerator ()
		{
			return albums.GetEnumerator ();
		}
		
		/*
		private string GetXmlForAdd (Album album)
		{
			StringWriter sr = new StringWriter ();
			writer = new XmlTextWriter (sr);
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 2;
			
			writer.WriteStartElement ("entry");
			writer.WriteAttributeString ("xmlns","http://www.w3.org/2005/Atom");
			writer.WriteAttributeString ("xmlns","f",null,"yandex:fotki");
			writer.WriteElementString ("title",album.title);
			writer.WriteElementString ("summary",album.summary);
			if (album.protect)
				writer.WriteElementString ("f:password",album.password);
			writer.WriteEndElement ();
			
			writer.Close ();
			return sr.ToString ();
		}
		*/
		
		private void ParseXml (XPathDocument doc)
		{
			var nav = doc.CreateNavigator ();
			var mr = new XmlNamespaceManager (nav.NameTable);
			mr.AddNamespace ("app","http://www.w3.org/2007/app");
			mr.AddNamespace ("f","yandex:fotki");
			mr.AddNamespace ("atom","http://www.w3.org/2005/Atom"); 
			
			XPathNodeIterator iterator = nav.Select ("/atom:feed/atom:entry",mr);
			while(iterator.MoveNext ()){
				albums.Add (new Album 
				            (new XPathDocument 
				             (iterator.Current.ReadSubtree())));
			}
		}
	}
}
