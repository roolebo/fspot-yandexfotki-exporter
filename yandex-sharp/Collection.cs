using System;
using System.Xml;

namespace Mono.Yandex.Fotki {

	public class AlbumCollection {
		Connection conn;
		
		public Collection (Connection conn)
		{
			this.conn = conn;
		}
		
		public Album Add (Album album)
		{	
		}
		
		private string GetXmlForAdd (string title, string summary, string password)
		{
			XmlUtil xml = XmlUtil ();
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
	}
}
