
using System;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Yandex.Fotki{

	public class Picture{
		public string title;
		public string author;
		public string access;
		public string id;
		public string image_src;
		public bool xxx;
		public bool disable_comments;
		public bool hide_original;
		
		public Picture (string xml)
		{
			ParseXml (xml);
		}
		
		private void ParseXml (string xml){			
			var sr = new StringReader (xml);
			var doc = new XPathDocument (sr);
			var nav = doc.CreateNavigator ();
			
			id = nav.Evaluate("substring-after('/entry/id',':photo:')");
			
			title = (string)nav.Evaluate ("/entry/title");
			author = (string)nav.Evaluate ("/entry/author/name");
			access = (string)nav.Evaluate ("/entry/f:access/@value");
			xxx = (bool)nav.Evaluate ("boolean(/entry/f:xxx/@value)");
			hide_original = (bool)nav.Evaluate ("/entry/f:hide_original/@value");
			disable_comments = (bool)nav.Evaluate ("/entry/f:disable_comments/@value");
			image_src = nav.SelectSingleNode ("/entry/content/@src");			
		}
	}
}