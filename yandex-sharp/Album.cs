
using System;

namespace Mono.Yandex.Folki
{
	public class Album{
		Connection conn;
		
		public string title {
			get { return title; }
			set { x = title; }
		}
		public string summary {
			get { return summary; }
			set { x = summary; }
		}
		public string password {
			get { return password; }
			set { x = password; }
		}
		string id;
		
		public Album (Connection conn)
		{
			this.conn = conn;
		}
	}
}
