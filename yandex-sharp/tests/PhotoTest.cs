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
                        uint response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, 0);
                }

                [Test]
                public void AddWithTitle ()
                {
                        uint response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo.Title = "Рябина под крышей";
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, 0);
                }

                [Test]
                public void AddWithDisabledComments ()
                {
                        uint response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo.DisableComments = true;
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, 0);
                }

                [Test]
                public void AddWithHiddenOriginal ()
                {
                        uint response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo.HideOriginal = true;
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, 0);
                }

                [Test]
                public void AddWithAdultContent ()
                {
                        uint response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo.AdultContent = true;
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, 0);
                }

                [Test]
                public void AddForFriends ()
                {
                        uint response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo.AccessLevel = Access.Friends;
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, 0);
                }

                [Test]
                public void AddForMyself ()
                {
                        uint response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo.AccessLevel = Access.Private;
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, 0);
                }

                [Test]
                public void AddWithAllProperties ()
                {
                        uint response;
                        Photo test_photo = new Photo ("testimg.jpg");
                        test_photo.Title = "Рябина под крышей";
                        test_photo.AccessLevel = Access.Private;
                        test_photo.AdultContent = true;
                        test_photo.HideOriginal = true;
                        test_photo.DisableComments = true;
                        response = photos.Add (test_photo);
                        Assert.AreNotEqual (response, 0);
                }
        }
}
