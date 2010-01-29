//  
//Mono.Yandex.Fotki.Authentication.cs: Handles authentication to Yandex.Fotki
//
//Author:
//  Roman Bolshakov
//
using System;
using System.Text;
using System.Net;
using System.Xml.XPath;
using System.IO;
using System.Collections.Generic;
using System.Web;
using Mono.Math;
//TODO catch and rethrow exceptions
namespace Mono.Yandex.Fotki {
    class Authentication {
        private const string auth_key_uri = "http://auth.mobile.yandex.ru/yamrsa/key/";
        private const string auth_token_uri = "http://auth.mobile.yandex.ru/yamrsa/token/";

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

        static string GetAuthorizationToken (string username, string password)
        {
            //getting public key
            System.Net.ServicePointManager.Expect100Continue = false;
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create (auth_key_uri);
            HttpWebResponse response = (HttpWebResponse) request.GetResponse ();
            XPathNavigator navigator = (new XPathDocument (new StreamReader (
                            response.GetResponseStream ()))).CreateNavigator ();
            string public_key = (string) navigator.Evaluate ("string(/response/key)");
            string request_id = (string) navigator.Evaluate ("string(/response/request_id)");
            response.Close ();
            
            //encoding
            EncryptionProvider encryption_provider = new EncryptionProvider ();
            encryption_provider.ImportPublicKey (public_key);
            string credentials = String.Format (@"<credentials login=""{0}"" password=""{1}""/>", username, password);
            string encoded_credentials = Convert.ToBase64String (encryption_provider.Encrypt 
                    (new UTF8Encoding ().GetBytes (credentials)));  
                    
            //sending encoded data and receiving authorization token
            request = (HttpWebRequest) WebRequest.Create (auth_token_uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] parameters = Encoding.UTF8.GetBytes ("request_id=" +
                    request_id + "&credentials=" + HttpUtility.UrlEncode(encoded_credentials));
            
            Stream request_stream = request.GetRequestStream ();
            request_stream.Write (parameters, 0, parameters.Length);
            request_stream.Close ();

            response = (HttpWebResponse) request.GetResponse ();
            navigator = (new XPathDocument (new StreamReader (
                            response.GetResponseStream ()))).CreateNavigator ();
            string token = (string) navigator.Evaluate ("string(/response/token)");
            response.Close ();

            return token;
        }
    }
}   
