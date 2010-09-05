SOURCES = Authentication.cs		\
	  tests/AuthenticationTest.cs	\
	  FotkiService.cs		\
	  tests/FotkiServiceTest.cs	\
	  RequestManager.cs		\
	  StreamHelper.cs		\
	  PhotoCollection.cs		\
	  FeedEnumerator.cs		\
	  PhotoCollectionEnumerator.cs	\
	  Photo.cs			\
	  MultipartData.cs		\
	  WebHelper.cs			\
	  UserNotFoundException.cs	\
	  AuthenticationFailedException.cs	\
	  ConnectionFailedException.cs	\
	  UploadPhotoCompletedEventArgs.cs	\
	  UploadPhotoCompletedEventHandler.cs	\
	  UploadPhotoProgressChangedEventArgs.cs	\
	  UploadPhotoProgressChangedEventHandler.cs	\
	  DownloadPhotoCompletedEventArgs.cs	\
	  DownloadPhotoCompletedEventHandler.cs	\
	  DownloadPhotoProgressChangedEventArgs.cs	\
	  DownloadPhotoProgressChangedEventHandler.cs	\
	  tests/PhotoTest.cs		\
	  DateTimeHelper.cs		\
	  tests/PhotoCollectionTest.cs

PHOTO_COLLECTION_TEST_SOURCES = Authentication.cs	\
				RequestManager.cs	\
				AuthenticationFailedException.cs	\
				ConnectionFailedException.cs	\
				UserNotFoundException.cs	\
				MultipartData.cs	\
				WebHelper.cs			\
				StreamHelper.cs			\
				tests/DownloadWholePhotoCollection.cs


all: FotkiSharp.dll DownloadWholePhotoCollection.exe

FotkiSharp.dll: $(SOURCES)
	gmcs -pkg:nunit -t:library -r:Mono.Security.dll,System.Web.dll -out:bin/$@ $^

DownloadWholePhotoCollection.exe : $(PHOTO_COLLECTION_TEST_SOURCES) 
	gmcs -t:exe -r:Mono.Security.dll,System.Web.dll -out:bin/$@ $^

clean:
	rm -rf FotkiSharp.dll
