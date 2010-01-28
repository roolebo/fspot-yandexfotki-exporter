//  
//Mono.Yandex.Fotki.Authentication.cs: Handles authentication to Yandex.Fotki
//
//Author:
//  Roman Bolshakov
//
using System;
using System.Text;
using System.Collections.Generic;
using Mono.Math;

namespace Mono.Yandex.Fotki {
    class Authentication {
        private class PublicKey {
            public BigInteger module;
            private BigInteger exponent;

            public PublicKey (string key)
            {
                string[] key_pair;
                key_pair = key.Split (new[] {'#'}, 2);
                if (key_pair.Length != 2)
                    throw new ArgumentException ("Incorrect value", "key");

                module = ParseHex (key_pair[0]);
                exponent = ParseHex (key_pair[1]);
            }

            private static BigInteger ParseHex (string hexString)
            {
                BigInteger result = new BigInteger ();
                foreach (char c in hexString) {
                    result = result * 16 + int.Parse (c.ToString (), 
                            System.Globalization.NumberStyles.AllowHexSpecifier);
                }

                return result;
            }

            public BigInteger Encrypt (BigInteger plain)
            {
                return plain.ModPow (exponent, module);
            }
        }

        private class EncryptionProvider {
            private PublicKey public_key;

            public void ImportPublicKey (string publicKeyString)
            {
                public_key = new PublicKey (publicKeyString);
            }

            public byte[] Encrypt (byte[] data)
            {
                int portion_len = (public_key.module.BitCount() - 1) / 8;
 
                byte[] prev_crypted = new byte[portion_len];
                List<byte> encrypted = new List<byte> ();

                for(int i = 0; i < ((data.Length - 1) / portion_len) + 1; i++) {// i - index of data portion
                    int cur_size = data.Length > portion_len ? portion_len : data.Length;

                    byte[] portion = new byte[cur_size];
                    for(int j = 0; j < cur_size; j++)
                        portion[j] = (byte) (data[i*portion_len + j] ^ prev_crypted[j]);

                    byte[] cipher = public_key.Encrypt (new BigInteger (portion)).GetBytes ();

                    Array.Copy (cipher, prev_crypted, portion_len);
                    Array.Clear (prev_crypted, portion_len, prev_crypted.Length - portion_len);
                    
                    encrypted.AddRange (BitConverter.GetBytes ((ushort) cur_size));
                    encrypted.AddRange (BitConverter.GetBytes ((ushort) cipher.Length));
                    encrypted.AddRange (cipher);
                }

                return encrypted.ToArray();
            }
            
        }

        public String GetAuthorizationToken()
        {
        }
        /*
        public static void Main()
        {
            EncryptionProvider encryption_provider = new EncryptionProvider ();
            string data = "test";

            encryption_provider.ImportPublicKey(
            "BFC949E4C7ADCC6F179226D574869CBF44D6220DA37C054C64CE48D4BAA36B039D8206E45E4576BFDB1D3B40D958FF0894F6541717824FDEBCEDD27C4BE1F057#10001");

            byte[] encoded = encryption_provider.Encrypt(Encoding.ASCII.GetBytes(data));
            Console.WriteLine(Convert.ToBase64String(encoded));
        }
        */
    }
}   
