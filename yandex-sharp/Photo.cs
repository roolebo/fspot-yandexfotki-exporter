
using System;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Text;

namespace Mono.Yandex.Fotki{

	public class Photo {
		public string title;
		public string author;
		public string id;
		public string filename;
		public bool xxx = false;
		public bool disable_comments = false;
		public bool hide_original = false;
		
		public Photo () {}
		
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
			XmlNamespaceManager mr = new XmlNamespaceManager (nav.NameTable);
			mr.AddNamespace ("app","http://www.w3.org/2007/app");
			mr.AddNamespace ("f","yandex:fotki");
			mr.AddNamespace ("atom","http://www.w3.org/2005/Atom");
			
			id = (string)nav.Evaluate("substring-after(/atom:entry/atom:id,':photo:')",mr);
			
			title = (string)nav.Evaluate ("string(/atom:entry/atom:title)",mr);
			author = (string)nav.Evaluate ("string(/atom:entry/atom:author/atom:name)",mr);
			xxx = bool.Parse((string)nav.Evaluate ("string(/atom:entry/f:xxx/@value)",mr));
			hide_original = bool.Parse((string)nav.Evaluate ("string(/atom:entry/f:hide_original/@value)",mr));
			disable_comments = bool.Parse((string)nav.Evaluate ("string(/atom:entry/f:disable_comments/@value)",mr));
			filename = (string)nav.Evaluate ("string(/atom:entry/atom:content/@src)",mr);			
		}
	}
}
