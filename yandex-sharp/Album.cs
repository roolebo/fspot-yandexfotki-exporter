
using System;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Yandex.Fotki
{
	public class Album:PhotoCollection {
		
		public string id = 0;
		public string title;
		public string author;
		public string summary;
		public bool protect;
		public int count = 0;
		public string password;
		
		public Album (XPathDocument xml)
		{
			ParseXml (xml);
		}
		
		public void Delete ()
		{
			
		}
		
		public void Update ()
		{}
		
		private void ParseXml (XPathDocument doc)
		{
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
