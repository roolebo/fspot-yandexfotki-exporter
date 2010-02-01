
using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections;

namespace Mono.Yandex.Fotki{

	public class Photo {
		public string title;
		public string author;
		public string id;
		public string filename;
		public bool xxx = false;
		public bool disable_comments = false;
		public bool hide_original = false;
		
		public Photo (XPathDocument xml)
		{
			ParseXml (xml);
		}
		
		/*
		public void Delete ()
		{
			RequestManager.Delete (this);
		}
		
		public byte[] Download ()
		{
			return RequestManager.Download (this);
		}
		
		public void Update ()
		{
			RequestManager.Edit (this);
		}
		*/
		
		private void ParseXml (XPathDocument doc){			
			var nav = doc.CreateNavigator ();
			
			id = (string)nav.Evaluate("substring-after('/entry/id',':photo:')");
			
			title = (string)nav.Evaluate ("/entry/title");
			author = (string)nav.Evaluate ("/entry/author/name");
			xxx = (bool)nav.Evaluate ("boolean(/entry/f:xxx/@value)");
			hide_original = (bool)nav.Evaluate ("/entry/f:hide_original/@value");
			disable_comments = (bool)nav.Evaluate ("/entry/f:disable_comments/@value");
			filename = (string)nav.Evaluate ("/entry/content/@src");			
		}
	}
}
