using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Yandex.Fotki{
	public class ServiceDocument{
		Connection conn;
		public string user;
		public string albums;
		public string photos;

		public ServiceDocument (Connection conn)
		{
			this.conn = conn;
		}

		public ServiceDocument (Connection conn, string user) : this(conn)
		{
			this.user = user;
		}
		
		public void RequestDocument ()
		{
		}

		private void ParseXml (string xml)
		{
			StringReader sr = new StringReader (xml);
			XPathDocument doc = new XPathDocument (sr);
			XPathNavigator nav = doc.CreateNavigator ();
			XPathExpression expr = nav.Compile ("/*/*/*/@href");
			XPathNodeIterator iterator = nav.Select (expr);
			iterator.MoveNext ();
			albums = iterator.Current.ToString ();
			iterator.MoveNext ();
			photos = iterator.Current.ToString ();
		}

	}
}