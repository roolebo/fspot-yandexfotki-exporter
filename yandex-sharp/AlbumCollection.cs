using System;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Yandex.Fotki {

	public class AlbumCollection {
		public string title;
		public string link;
		
		public void Add (Album album)
		{
			
		}
		
		public void Edit (Album album)
		{
			
		}
		
		public void Delete (Album album)
		{
			
		}
		
		private string GetXmlForAdd (Album album)
		{
			XmlUtil xml = new XmlUtil ();
			xml.WriteStartElement ("entry");
			xml.WriteAttributeString ("xmlns","http://www.w3.org/2005/Atom");
			xml.WriteAttributeString ("xmlns","f",null,"yandex:fotki");
			xml.WriteElementString ("title","Заголовок");
			xml.WriteElementString ("summary","Описание альбома");
			if (password == null || password == String.Empty)
				xml.WriteElementString ("f:password","123");
			xml.WriteEndElement ();
			
			return xml.GetDocumentString ();
		}
		
		private void ParseXml (XPathDocument doc)
		{
			var nav = doc.CreateNavigator ();
			
			title = (string)nav.Evaluate ("/feed/title");
			link = (string)nav.Evaluate ("/feed/link[rel='alternative']");
			var iterator = nav.Select ("/feed/entry");
			foreach (XPathNodeIterator item in iterator){
				//Создание альбомов
			}
		}
	}
}
