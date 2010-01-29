
using System;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace Mono.Yandex.Fotki
{
	public class Album:PhotoCollection {
		
		public string id;
		public string title;
		public string author;
		public string summary;
		public bool protect = false;
		public int count;
		public string password;
		
		public Album (XPathDocument xml):base (xml)
		{
			ParseXml (xml);
		}
		
		public void Delete ()
		{
			RequestManager.Delete (this);
		}
		
		public void Update ()
		{
			RequestManager.Edit (this);
		}
		
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
		
		public static void Main (){
			
			string xml = @"<feed xmlns='http://www.w3.org/2005/Atom' xmlns:app='http://www.w3.org/2007/app' xmlns:f='yandex:fotki'>
					  <id>urn:yandex:fotki:maxum:album:63988:photos</id>
					  <author>
					    <name>maxum</name>
					  </author>
					  <title>Каникулы в Германии</title>
					  <updated>2009-01-27T11:57:33Z</updated>
					  <summary>Наша школьная поездка в Германию</summary>
					  <link href='http://api-fotki.yandex.ru/api/users/maxum/album/63988/photos/' rel='self' />
					  <link href='http://fotki.yandex.ru/users/maxum/album/63988/' rel='alternate' />
					  <f:image-count value='10' />
					  <entry>
					    <id>urn:yandex:fotki:maxum:photo:126746</id>
					    <title>Я и Саша на фоне Бранденбургских ворот</title>
					    <author>
					      <name>maxum</name>
					    </author>
					    <link href='http://api-fotki.yandex.ru/api/users/maxum/photo/126746/' rel='self' />
					    <link href='http://api-fotki.yandex.ru/api/users/maxum/photo/126746/' rel='edit' />
					    <link href='http://fotki.yandex.ru/users/maxum/view/126746/' rel='alternate' />
					    <link href='http://img-fotki.yandex.ru/get/3306/maxum.0/0_1ef1a_9a6e7171_XL' rel='edit-media' />
					    <link href='http://api-fotki.yandex.ru/api/users/maxum/album/63988/' rel='album' />
					    <published>2009-01-27T11:57:32Z</published>
					    <app:edited>2009-01-27T11:57:32Z</app:edited>
					    <updated>2009-01-27T11:57:32Z</updated>
					    <f:created>2008-07-10T22:02:40</f:created>
					    <f:access value='private' />
					    <f:xxx value='false' />
					    <f:hide_original value='false' />
					    <f:disable_comments value='false' />
					    <content src='http://img-fotki.yandex.ru/get/3306/maxum.0/0_1ef1a_9a6e7171_XL' type='image/*' />
					  </entry>
					</feed>";
			
			Album a = new Album (new XPathDocument (new StreamReader (xml)));
		}
	}
}
