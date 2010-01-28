
using System;

namespace Mono.Yandex.Fotki{

	public class YandexFotki{

		public YandexFotki ()
		{
		}
		
		public static void Main ()
		{
			ServiceDocument doc = new ServiceDocument (new Connection ());
			doc.RequestDocument ();
		}
	}
}
