
using System;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Yandex.Fotki
{
	public class Album{
		public string id;
		public string title;
		public string author;
		public string summary;
		public bool protect;
		public int count;
		
		public Album (string aid)
		{
			this.id = aid;
		}
		
		public Album (string xml)
		{
			ParseXml (xml);
		}
		
		private void ParseXml (string xml)
		{
			var sr = new StringReader (xml);
			var doc = new XPathDocument ();
			var nav = doc.CreateNavigator ();
			id = (string)nav.Evaluate ("substring-after('/entry/id',':album:')");
			title = (string)nav.Evaluate ("/entry/title");
			author = (string)nav.Evaluate ("/entry/author/name");
			summary = (string)nav.Evaluate ("/entry/summary");
			protect = (bool)nav.Evaluate ("boolean(/entry/f:protected/@value)");
			count = (int)nav.Evaluate ("integer(/entry/f:image-count/@value)");			
		}
	}
}
