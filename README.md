BlinkStick Client
=================

This is BlinkStick Client application repository.

What is BlinkStick? It's a DIY USB RGB LED device. More info about it here:

http://www.blinkstick.com

Binary releases
---------------

Binary releases are available here:

http://www.blinkstick.com/help/downloads

Overview
--------

BlinkStick Client application is written in Mono/.NET 4.0 C# using Gtk#.

Current full list of features in the client application:

* Control any number of BlinkSticks
* Detects when device is plugged in or unplugged.
* Check email and get notifications about new email messages
* Connect to BlinkStick.com and control the LED remotely
* Visually display CPU usage
* Use BlinkStick as AmbiLight clone

How to build (Windows)
----------------------

* Download and install [Microsoft .NET 4.0 Full](http://www.microsoft.com/en-gb/download/details.aspx?id=17718)
* Download and install [GTK# for .NET 2.12.20](http://download.xamarin.com/Installer/gtk-sharp-2.12.20.msi)
* Download and install [Xamarin Studio 4](http://monodevelop.com/Download)
* All dependant libraries are inside the repository

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

_Will be written once source code is compatible with Mac OSX._

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
