# Gips

Gips is a Mac app and finder extension that handles basic image processing such as resizing.

## Open sourcing Gips

Gips is a gui wrapper around [sips (scriptable image processing system)](https://www.google.ca/url?sa=t&rct=j&q=&esrc=s&source=web&cd=1&cad=rja&uact=8&ved=0ahUKEwiqsKqw6bDYAhXm54MKHUteCQIQFggpMAA&url=https%3A%2F%2Fdeveloper.apple.com%2Flegacy%2Flibrary%2Fdocumentation%2FDarwin%2FReference%2FManPages%2Fman1%2Fsips.1.html&usg=AOvVaw2vXZ3FxyN6B4zPCB2JDSrp) – a command line tool on OS X that allows you to resize photos. I built Gips because:

1. As I helped migrate some long time Windows folks over to OS X, some of them photographers, I realized that they were missing the ImageMagick image resampling app that was commonly available on Windows, so Gips was intended to provide some of the same functionality
2. I wanted to learn native OSX development in Objective-C and thought this would be a fun project to go around tinkering with native development. 

I had originally planned on releasing Gips on the app store at a small price point, however, at some point, I stopped active development on this project, still using what I had built on a fairly regular basis. 

I've decided to open source in the hopes that people looking for a similar tool have access to it, and potentially see if other folks want to build on top of and improve this project. 

### Next steps

Over the next couple of weeks, time permitting, I want to look into the following:
- Publishing a build for the app via Github, for people to use directly
- Better documenting the existing codebase, dev workflow

