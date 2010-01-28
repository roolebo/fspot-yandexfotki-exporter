
using System;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Yandex.Fotki{

	public class Picture{
		string title;
		string author;
		string access;
		string id;
		string image_src;
		bool xxx;
		bool disable_comments;
		bool hide_original;
		
		public Picture ()
		{
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