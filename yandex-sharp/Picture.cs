
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
			id = nav.Select ("//id").Current;
			id = id.Substring (id.LastIndexOf (':'));
			title = nav.Select ("//title").Current;
			author = nav.Select ("//author/name").Current;
			access = nav.Select ("//f:access/@value").Current;
			xxx = nav.Select ("//f:xxx/@value").Current.ValueAsBoolean;
			hide_original = nav.Select ("//f:hide_original/@value")
				.Current.ValueAsBoolean;
			disable_comments = nav.Select ("//f:disable_comments/@value")
				.Current.ValueAsBoolean;
			image_src = nav.Select ("//content/@src").Current;			
		}
	}
}