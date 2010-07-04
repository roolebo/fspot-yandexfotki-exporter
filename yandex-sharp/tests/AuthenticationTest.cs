namespace Mono.Yandex.Fotki {
        using System;
        using System.IO;
        using NUnit.Framework;

        [TestFixture]
        public class AuthenticationTest {
                String[] login_config = new String[2];

                [SetUp]
                public void Init ()
                {
                        //the first line of config file is the login, 
                        //and the second one is the password
                        using (StreamReader sr = new StreamReader ("config")) {   
                                login_config[0] = sr.ReadLine ();
                                login_config[1] = sr.ReadLine ();
                        }
                }

                [Test]
                public void GetToken ()
                {
                        String token = Authentication.GetAuthorizationToken 
                                (login_config[0], login_config[1]);
                        Assert.AreNotEqual(token, null);
                }
                
                [Test]
                [ExpectedException("System.Net.WebException")]
                public void GetTokenWithoutPassword ()
                {
                        String token = Authentication.GetAuthorizationToken
                                (login_config[0], null);
                        Assert.AreEqual(token, null);
                }
        }
}
