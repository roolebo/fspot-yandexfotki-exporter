
using System;

namespace Mono.Yandex.Fotki
{
	public class Album{
		public string id;
		public string title;
		public string summary;
		public string password;
		
		public Album (string aid)
		{
			this.id = aid;
		}
	}
}
