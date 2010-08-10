SOURCES = Authentication.cs		\
	  tests/AuthenticationTest.cs	\
	  FotkiService.cs		\
	  tests/FotkiServiceTest.cs	\
	  RequestManager.cs		\
	  StreamHelper.cs		\
	  PhotoCollection.cs		\
	  Photo.cs			\
	  MultipartData.cs		\
	  WebHelper.cs			\
	  UserNotFoundException.cs	\
	  AuthenticationFailedException.cs	\
	  ConnectionFailedException.cs

all: FotkiSharp.dll

FotkiSharp.dll: $(SOURCES)
	gmcs -pkg:nunit -t:library -r:Mono.Security.dll,System.Web.dll,System.Drawing.dll -out:bin/$@ $^

clean:
	rm -rf FotkiSharp.dll
