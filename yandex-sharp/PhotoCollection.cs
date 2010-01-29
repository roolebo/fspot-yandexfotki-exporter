
using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace Mono.Yandex.Fotki{
	
	public class PhotoCollection:IEnumerable {
		public string link;
		private ArrayList photos;

		public PhotoCollection (XPathDocument xml)
		{
			ParseXml (xml);
		}
		
		public void Upload (Photo photo)
		{
			RequestManager.Add (photo);
		}
		
		public IEnumerator GetEnumerator ()
		{
			foreach (Photo photo in photos)
			{
				yield return photo;
			}
		}
		
		private void ParseXml (XPathDocument xml)
		{
			var nav = xml.CreateNavigator ();
			
			var iterator = nav.Select ("//entry");
			foreach (XPathNodeIterator item in iterator){
				albums.Add (new Album (new XPathDocument 
				                       (new StringReader (item))));
			}
		}
		
	}
}
