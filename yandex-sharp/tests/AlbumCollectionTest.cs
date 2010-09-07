using System;
using System.IO;
using NUnit.Framework;
namespace Mono.Yandex.Fotki {
        [TestFixture]
        public class AlbumCollectionTest {
                String[] login_config = new String[2];
                FotkiService fotki;
                AlbumCollection albums;

                [SetUp]
                public void Init ()
                {
                        //the first line of config file is the login,
                        //and the second one is the password
                        using (StreamReader sr = new StreamReader ("config")) {
                                login_config [0] = sr.ReadLine ();
                                login_config [1] = sr.ReadLine ();
                        }

                        fotki = new FotkiService (login_config [0], login_config [1]);
                        albums = fotki.GetAlbums ();
                }

                [Test]
                public void ForeachTest ()
                {
                        foreach (Album a in albums) {
                                Console.WriteLine (a.Published);
                        }
                }
        }
}
