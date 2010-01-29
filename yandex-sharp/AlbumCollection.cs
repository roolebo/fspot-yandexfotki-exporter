using System;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Text;
using System.Collections;

namespace Mono.Yandex.Fotki {

	public class AlbumCollection:IEnumerable {
		public string link;
		private ArrayList albums;
		
		public AlbumCollection (XPathDocument xml)
		{
			this.albums = new ArrayList ();
			ParseXml (xml);
		}
		
		public void Add (Album album)
		{
			RequestMananger.Add (album);
		}
		
		public IEnumerable GetEnumerator ()
		{
			foreach (Album album in albums)
			{
				yield return album;
			}
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
			
			title = (string)nav.Evaluate ("/feed/title");
			link = (string)nav.Evaluate ("/feed/link[rel='alternative']");
			var iterator = nav.Select ("/feed/entry");
			foreach (XPathNodeIterator item in iterator){
				albums.Add (new Album (new XPathDocument 
				                       (new StringReader (item))));
			}
		}
	}
}
