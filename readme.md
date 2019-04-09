# AstraDotNet Demo

This is a simple .Net solution to demonstrate working with [Orbbec Astra (Pro)](https://orbbec3d.com/product-astra/) depth sensors from C#.

### Features:

* Visualization of depth and color streams
* Visualization of body tracking (as stick skeletons over depth map)
* Calculation of actual frame rate (FPS, frames per second)
* Support for multiple sensors connected to one PC
* Registration mode switcher for depth stream (registration allows to align depth map with color image)
* Any CPU (support for both 32-bit and 64-bit architectures)


### How to use:

* Open `AstraDotNetDemo.sln` in Visual Studio 2017
* Build and run `AstraTestWpf` application (F5)
* All required binaries and libraries are already in repository (see `externals` folder) and are copied automatically to output directory during build


### Known issues in Astra SDK:

1. For Astra PRO actual frame rate is not exactly 30 FPS. It is about 29.7 FPS for depth only mode, and about 28.5 FPS for depth+color mode.
2. When more than one sensor is connected to one and the same PC, the following limitations take place:
 * For Astra PRO sensors: lowering of frame rate to 15 FPS if color stream is on
 * For Astra PRO sensors: you can see live color stream only from one sensor (black screen for other sensors)
 * Body (skeletal) tracking doesn't work if it is turned on for more than one sensor
3. Wrong information about field-of-view for color stream.
4. Donot unplug sensor while application is running. In other case application may crash on exit.


### Stuff used to make this:

 * [.Net Framework 4.6.1](https://www.microsoft.com/en-us/download/details.aspx?id=49981)
 * [Visual Studio 2017](https://www.visualstudio.com/downloads/) (Commutiny Edition)
 * [Astra SDK v2.0.15](https://orbbec3d.com/develop/) (win32 and win64 versions for VS 2015)


### License:

Astra SDK is licensed under Apache v2.0 and includes components of OpenNI and OpenCV projects.

This sample code is licensed under [MIT license](https://opensource.org/licenses/MIT).