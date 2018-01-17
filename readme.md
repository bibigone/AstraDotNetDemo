# AstraDotNet Demo

This is a simple .Net solution to demonstrate working with [Orbbec Astra (Pro)](https://orbbec3d.com/product-astra/) depth sensors from C#.

### Features:

* Visualization of depth and color streams
* Visualization of body tracking (as stick skeletons over depth map)
* Calculation of actual FPS
* Support for multiple sensors connected to one PC


### How to use:

* Open `AstraDotNetDemo.sln` in Visual Studio 2017
* Build and run `AstraTestWpf` application (F5)
* All required binaries and libraries are already in repository (see `externals` folder)


### Stuff used to make this:

 * [.Net Framework 4.6.1](https://www.microsoft.com/en-us/download/details.aspx?id=49981)
 * [Visual Studio 2017](https://www.visualstudio.com/downloads/) (Commutiny Edition)
 * [Astra SDK v2.0.7](https://orbbec3d.com/develop/) (win32 and win64 versions for VS 2015)



### License:

Astra SDK is licensed under Apache v2.0 and includes components of OpenNI and OpenCV projects.

This sample code is licensed under MIT license.