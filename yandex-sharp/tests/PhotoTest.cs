namespace Mono.Yandex.Fotki {
        using System;
        using System.IO;
        using NUnit.Framework;

        [TestFixture]
        public class PhotoTest {
                String[] login_config = new String[2];
                FotkiService fotki;
                PhotoCollection photos;

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
                        photos = fotki.GetPhotos ();
                }

                [Test]
                public void AddOnlyImage ()
                {
                        Photo response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, null);
                }

                [Test]
                public void AddWithTitle ()
                {
                        Photo response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo.Title = "Рябина под крышей";
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, null);
                }

                [Test]
                public void AddWithDisabledComments ()
                {
                        Photo response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo.DisableComments = true;
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, null);
                }

                [Test]
                public void AddWithHiddenOriginal ()
                {
                        Photo response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo.HideOriginal = true;
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, null);
                }

                [Test]
                public void AddWithAdultContent ()
                {
                        Photo response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo.AdultContent = true;
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, null);
                }

                [Test]
                public void AddForFriends ()
                {
                        Photo response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo.AccessLevel = Access.Friends;
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, null);
                }

                [Test]
                public void AddForMyself ()
                {
                        Photo response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo.AccessLevel = Access.Private;
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, null);
                }

                [Test]
                public void AddWithAllProperties ()
                {
                        Photo response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo.Title = "Рябина под крышей";
                        test_photo.AccessLevel = Access.Private;
                        test_photo.AdultContent = true;
                        test_photo.HideOriginal = true;
                        test_photo.DisableComments = true;
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, null);
                }

                [Test]
                public void UpdateTitle ()
                {
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo = photos.Add (test_photo);
                        test_photo.Title = "Изменённое название";
                        test_photo.Update ();
                }

        }
}
