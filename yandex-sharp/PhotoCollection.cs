
using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Yandex.Fotki{
	
	public class PhotoCollection{
		private ArrayList photos;

		public PhotoCollection ()
		{
		}
		
		public void Add (Photo photo)
		{}
		
		public void Edit (Photo photo)
		{}
		
		public void Delete (Photo photo)
		{}
		
		private void ParseXml (XPathDocument xml)
		{
			var nav = xml.CreateNavigator ();
			
			title = (string)nav.Evaluate ("/feed/title");
			link = (string)nav.Evaluate ("/feed/link[rel='alternative']");
			var iterator = nav.Select ("/feed/entry");
			foreach (XPathNodeIterator item in iterator){
				albums.Add (new Album (new XPathDocument 
				                       (new StringReader (item))));
			}
		}
	}
}
