using System;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Yandex.Fotki {

	public class AlbumCollection {
		public string title;
		public string link;
		
		private string GetXmlForAdd (string title, string summary, string password)
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
		
		private void ParseXml (string xml)
		{
			var sr = new StringReader (xml);
			var doc = new XPathDocument (sr);
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
