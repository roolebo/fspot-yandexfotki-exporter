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
			var sr = new StringReader (xml);
			var doc = new XPathDocument (sr);
			var nav = doc.CreateNavigator ();
			var iterator = nav.Select("//app:collection/@href");
			albums = iterator.Current.ToString ();
			iterator.MoveNext ();
			photos = iterator.Current.ToString ();
		}
	}
}