
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace Mono.Yandex.Fotki{
	
	public class PhotoCollection{
		public string link;
		private ArrayList photos;

		public PhotoCollection (XPathDocument xml)
		{
			photos = new ArrayList ();
			ParseXml (xml);
		}
		/*
		public void Upload (Photo photo)
		{
			RequestManager.Add (photo);
		}
		*/
		
		public IEnumerator GetEnumerator ()
		{
			return photos.GetEnumerator();
		}
		
		private void ParseXml (XPathDocument xml)
		{
			var nav = xml.CreateNavigator ();
			var mr = new XmlNamespaceManager (nav.NameTable);
			mr.AddNamespace ("app","http://www.w3.org/2007/app");
			mr.AddNamespace ("f","yandex:fotki");
			mr.AddNamespace ("atom","http://www.w3.org/2005/Atom");
			
			link = (string)nav.Evaluate ("string(//atom:link[@rel='alternate'][1]/@href)",mr);
			
			XPathNodeIterator iterator = nav.Select ("/atom:feed/atom:entry",mr);
			while(iterator.MoveNext ()){
				photos.Add (new Photo 
				            (new XPathDocument 
				             (iterator.Current.ReadSubtree())));
			}
		}
	}
}
