![BlinkStick Client](https://www.blinkstick.com/system/resources/logos/blinkstick-client.png)

This is client application repository for BlinkStick smart LED controllers with integrated USB firmware. More info about it here:

http://www.blinkstick.com

Binary releases
---------------

Binary releases are available here:

http://www.blinkstick.com/help/downloads

Release candidates are available in forums:

https://forums.blinkstick.com/

Overview
--------

![BlinkStick Client Screenshot](http://www.blinkstick.com/system/resources/screenshots/blinkstick-client.png)

BlinkStick Client application is written in Mono/.NET 4.0 C# using Gtk#.

Current full list of features in the client application:

* Supports all BlinkStick devices (Original BlinkStick, Pro, Strip, Square, Flex and Nano)
* Supports all LEDs on devices
* Completely rewritten LED animation engine
* Completely rewritten ambilight notification with support for DirectX games
* New notification types include Moodlight, Application, RAM, Battery, Disk space and ability to remotely control BlinkStick client application via HTTP requests
* Customization of patterns and pattern animations
* Ability to select and test LEDs in the GUI for all supported devices

How to build (Windows)
----------------------

* Download and install [Microsoft .NET 4.0 Full](http://www.microsoft.com/en-gb/download/details.aspx?id=17718)
* Download and install [Gtk# 2.12.45]https://xamarin.azureedge.net/GTKforWindows/Windows/gtk-sharp-2.12.45.msi
* Download and install [Xamarin Studio 5](http://download.xamarin.com/studio/Windows/XamarinStudio-5.9.5.9-0.msi)
* All dependant libraries are inside the repository or will be restored automatically with NuGet

Clone this repository using Git.

```
git clone https://github.com/arvydas/blinkstick-client.git
```

Open BlinkStick.sln in Xamarin Studio and do a _Build -> Build All_, then _Run -> Start Debugging_.

*Note:* You can also open and compile the solution file in Microsoft Visual Studio 2010 and up, but 
you will not be able to desgin any Gtk# forms/dialogs, because Visual Studio does not support them.

How to build (Linux)
--------------------

You will need a recent version of Linux distribution, 
for example [Linux Mint 14](http://www.linuxmint.com/) or [Ubuntu](http://www.ubuntu.com/).

```
sudo add-apt-repository ppa:keks9n/monodevelop-latest && sudo apt-get update && sudo apt-get install monodevelop-latest
```

Install libusb:

```
sudo apt-get install libusb-1.0-0-dev
```

Clone this repository using Git.

```
git clone https://github.com/arvydas/blinkstick-client.git
```

Open BlinkStick.sln in MonoDevelop 4.0 and do a _Build -> Build All_, then _Run -> Start Debugging_.

How to build (Mac OSX)
----------------------

* Download and install [Xamarin Studio 5](http://monodevelop.com/Download)
* All dependant libraries are inside the repository or will be restored automatically with NuGet

Clone this repository using Git.

```
git clone https://github.com/arvydas/blinkstick-client.git
```

Open BlinkStick.sln in Xamarin Studio and do a _Build -> Build All_, then _Run -> Start Debugging_.

Development
-----------

Join the development of BlinkStick Client application! Here is how you can contribute:

* Fork this repository
* Write some awesome code
* Issue a pull request

License
-------

BlinkStick Client application is licensed under GPL v3. Please contact for other 
licensing options if required.

Maintainer
----------

* Arvydas Juskevicius - [http://twitter.com/arvydev](http://twitter.com/arvydev)
