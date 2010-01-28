using System;
using Mono.Math;

namespace Mono.Yandex.Fotki{
	class Authentication{
		private class PublicKey
		{
			private BigInteger module, exponent;

			PublicKey (string key)
			{
				string[] key_pair;
				key_pair = key.Split (new[] { '#' }, 2);
				if (key_pair.Lenght != 2)
					throw new ArgumentException ("Incorrect value", "key");

				module = ParseHex (key_pair[0]);
				exponent = ParseHex (key_pair[1]);
			}

			private static BigInteger ParseHex (string hexString)
			{
				BigInteger result = new BigInteger ();
				foreach (char c in hexString) {
					result = result * 16 + int.Parse (c.ToString (), System.Globalization.NumberStyles.AllowHexSpecifier);
				}

				return result;
			}

			public BigInteger Encrypt (BigInteger plain)
			{
				return plain.ModPow (exponent, module);
			}
		}
	}
}
