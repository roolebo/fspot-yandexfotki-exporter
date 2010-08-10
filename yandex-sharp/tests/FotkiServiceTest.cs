namespace Mono.Yandex.Fotki {
        using System;
        using System.IO;
        using NUnit.Framework;

        [TestFixture]
        public class FotkiServiceTest {
                string [] config = new string [2];
                string [] wrong_config = new string [2];
                FotkiService fotki;

                [SetUp]
                public void Init ()
                {
                        //the first line of config file is the login, 
                        //and the second one is the password
                        using (var sr = new StreamReader ("config")) {   
                                config [0] = sr.ReadLine ();
                                config [1] = sr.ReadLine ();
                        }
                        using (var sr = new StreamReader ("wrongconfig")) { 
                                wrong_config [0] = sr.ReadLine ();
                                wrong_config [1] = sr.ReadLine ();
                        }
                }

                [Test]
                public void CreateFotkiServiceWithPassword ()
                {
                        fotki = new FotkiService (config [0], config [1]);
                }

                [Test]
                public void CreateFotkiServiceWithLoginOnly ()
                {
                        fotki = new FotkiService (config [0]);
                }

                [Test]
                [ExpectedException("Mono.Yandex.Fotki.AuthenticationFailedException")]
                public void CreateFotkiServiceWithWrongPassword ()
                {
                        fotki = new FotkiService (config [0], wrong_config [1]);
                }

                [Test]
                [ExpectedException("Mono.Yandex.Fotki.UserNotFoundException")]
                public void CreateFotkiServiceWithWrongLogin ()
                {
                        fotki = new FotkiService (wrong_config [0]);
                }
                
                [Test]
                public void GetPhotos ()
                {
                        fotki = new FotkiService (config [0]);
                        PhotoCollection photos;
                        photos = fotki.GetPhotos ();
                }
        }
}
