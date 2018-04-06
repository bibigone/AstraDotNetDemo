Astra SDK v2.0.9-beta3
Copyright (c) 2015-2018 Orbbec
https://www.orbbec3d.com

For help and support, visit us at https://3dclub.orbbec3d.com.

Features
========
* Depth stream
* Point stream - an image where each pixel is the real-world 3D point, i.e. depth to world
* Color stream - Astra, Astra Pro, and Persee
* Infrared stream - can start when color is stopped
* Body stream - features include:
** Full-body skeleton tracking - Tracks 6 people max, though performance may vary
** Body mask
** Floor mask and floor plane
** Hand pose recognition: Grip (fist) and other (open hand)
** Note: when the Body stream is started, the depth stream automatically goes into
   registered depth mode so that depth & color line up
* Masked Color stream - Background removal on the color frame using the body mask
* Colorized Body stream - Body mask visualized with different colors for each body
* Hand stream - separate from body tracking and uses less CPU: wave your hand to track a single hand point

Supported systems
=================
* Windows 8 and 10, 32-bit and 64-bit
* Ubuntu 14.04 64-bit
* Ubuntu 16.04 64-bit
* Android 4.4.2 and 5.1 (armeabi-v7a 32-bit)
* Orbbec Persee

Supported languages & frameworks
================================
* C
* C++
* .NET/C#
* Java
* Unity 5 - sample provided for Unity 5.3.6

Possibly working systems - these may work but are untested/unsupported at this time
========================
* Android 6.0, Android 7.0
* Unity3D 2017, Unity 4.7

Orbbec Body Tracking trial time expiration
===============
For this beta release, Orbbec Body Tracking expires on 2018/06/30
and will stop operating. Please make sure to update to a new version before then.
You can now set your license string to extend the trial. See API notes below.

For support on the trial expiration or to extend your trial, please contact
info@orbbec3d.com.

What's New
==========

v2.0.9-beta3 2018/03/31

* Add missing astra_jni.dll for Java wrapper which was omitted from previous release.
* Add body tracking frame skipping for very low-end CPUs (usually Android devices):
  If the calculation of a body frame is taking too long, it will estimate the next
  frame joints based upon the velocity of previous frames. The frame calculation
  will continue in the background and the results will be incorporated in a future
  frame. If the CPU is fast enough then this feature will not change body tracking
  behavior.
* Add isEstimated to determine if the body frame has been estimated due to extremely slow CPU.
  Typically this will only happen on very low-end Android devices.
  C: astra_bodyframe_info_t.isEstimated
  C++: BodyFrameInfo::is_estimated()
  C#: BodyFrameInfo.IsEstimated
  Java: BodyFrameInfo.getIsEstimated()
* astra_imagestream_get_usb_info()/ColorStream::usb_info() is now supported on
  Astra Pro color stream
* Minor improvements to the public headers (fixed some missing includes)
* Fix Android support on certain boxes: crashes, Astra Pro support
* Android will now write Astra SDK logs to the Android log (logcat)
* Minor improvements to error checking in the sample projects
* Fix issue with BodyStream that prevented it from starting again after it was
  previously started and then stopped.
* Slightly reduce RAM usage and improve startup time of body tracking
* Fix issue with SimpleBodyViewer-SFML running at low FPS by changing depth resolution
  from 160x120 to 640x480. (320x240 would also work.) There is a bug in some Astra
  firmwares that causes 160x120 to run at a slower framerate.
* Fix issue with color stream not working on Astra Mini
* Fix issue where MaskedColorStream would crash when user was farther than 2m
  when using an Astra camera with MX400 chip.

v2.0.8-beta2 2018/01/31

Full list of changes upcoming with final v2.0 release. This version has some small
breaking API changes. Be aware, there will be more breaking API changes between
now and the final v2.0 release.

New features:
* On Android with UVC color (Astra Pro & Persee), the API now supports listing the
  available resolutions, getting the current resolution, and setting the resolution.
  For low-end Android devices we recommend running color at 320x240 for performance
  reasons.
* Add body tracking feature API that lets you control the processing done for a tracked body:
  just segmentation, joints, hand poses. Each level includes the previous (hand poses
  includes joints and joints includes segmentation.) You can set the default for
  new bodies as well as change how a body is tracked for future frames.
  C: astra_body_t has the astra_body_features_t features field, which reflects the
     features used to generate the astra_body_t.
     astra_bodystream_get_body_features(), astra_bodystream_set_body_features(),
     astra_bodystream_get_default_body_features(), astra_bodystream_set_default_body_features()
  C++: Body::joints_enabled(), Body::hand_poses_enabled(),
       BodyStream::get_body_features(), BodyStream::set_body_features(),
       BodyStream::get_default_body_features(), BodyStream::set_default_body_features()
  C#: Body.AreJointsEnabled, Body.AreHandPosesEnabled,
      BodyStream.GetBodyFeatures(), BodyStream.SetBodyFeatures(),
      BodyStream.GetDefaultBodyFeatures(), BodyStream.SetDefaultBodyFeatures()
  Java: Body.areJointsEnabled(), Body.areHandPosesEnabled(),
        BodyStream.getBodyFeatures(), BodyStream.setBodyFeatures(),
        BodyStream.getDefaultBodyFeatures(), BodyStream.setDefaultBodyFeatures()
* Add license API:
  C/C++: orbbec_body_tracking_set_license(const char* licenseString)
  C#: Astra.BodyTracking.SetLicense(String licenseString)
  Java: BodyTracking.setLicense(String licenseString)
  Call these methods directly after astra_initialize() with the license key string
  provided to you by Orbbec.
* Orbbec Body Tracking trial expiration set to March 31, 2018
* Add Chinese translation of Astra Book documentation in docs/html-zh_CN
* Java and C# APIs updated to match features of C++ API
* MaskedColor calculation is now 3x faster
* C#: FloorMask now has CopyData() and IntPtr DataPtr to reduce memory allocations
* C++: Fixed bug where Neck joint was id 19 when it should have been id 18
* OpenNI2 backend updated with MX6000 chip support
* Various bug fixes

v2.0.7-beta 2017/12/16

Full list of changes upcoming with final v2.0 release. This version has some small
breaking API changes. Be aware, there will be more breaking API changes between
now and the final v2.0 release.

New features:
* .NET/C# wrapper
* Java wrapper
* Unity wrapper - distributed separately
* Color support for Astra Pro and Orbbec Persee
* Orbbec Body Tracking integration
* MaskedColorStream
* ColorizedBodyStream
* Various bug fixes
* Support for Android 4.4.2 & 5.1 - distributed separately

v0.5.0 2016/04/26

This release cleans up the API and library organization a bit. There are a few breaking changes in this release
from v0.4 but they should be relatively quick to update existing code.

* BREAKING: Library names:
  * astra -> astra_core
  * astraul -> astra
  * astra_api -> astra_core_api
* BREAKING: C++ API stylistic changes: standardize on namespace::ClassName::method_name
* BREAKING: Besides the predictable stylistic changes, a few C++ method renames:
  * DepthStream/ColorStream/etc horizontalFieldOfView -> hFov, verticalFieldOfView -> vFov
  * DepthFrame/ColorFrame/etc resolutionX/resolutionY -> width/height
  * DepthFrame/ColorFrame/etc numberOfPixels -> length
* BREAKING: C++ header filenames renamed according to namespace.hpp or Class.hpp.
  * Main header to include: <Astra/Astra.h> & <AstraUL/AstraUL.h> -> just <astra/astra.hpp>.
  * Don't need to explicitly include astra_core.hpp.
* BREAKING: astra::Astra::{initialize(),terminate()} -> astra::{initialize(),terminate()}
* Fix: Cycling start/stop on a stream multiple times no longer crashes
* Fix: SXGA depth & color support now work
* Enhancement: (ALL) Add const as appropriate in the C++ API, makes passing around references to frame types possible
* Enhancement: (ALL) StreamSet and StreamReader have improved copy-semantics and default ctor,
                     allowing simpler storage as a class field.
* Enhancement: (ALL) Add MultiSensorViewer-SFML sample demonstrating multi-sensor support
* Enhancement: (ALL) Removed the dependency on OpenCV and reimplemented the necessary functionality internally.
* Enhancement: (Windows) Add VS2015 support
* Enhancement: (OSX) libusb, SFML are now distributed with the SDK, and rpaths are automatically setup.
                     No more brew installing! (except maybe CMake)
* Enhancement: (OSX) Add build_samples.sh script in samples/, see ./build_samples.sh -h for help
* Enhancement: (OSX) Building the samples also copies the Astra SDK and SFML binaries to the lib dir,
                     allowing simpler copy-only deployment: Just copy bin/ and lib/.

v0.4.0 2015/10/14

* Add official support for Win64 and OS X 10.8+
* Updated SFML to 2.3.2
* Added features to SimpleDepthViewer-SFML and SimpleStreamViewer-SFML: pausing,
  overlay color on depth, display depth data under the mouse in text overlay. (See keyboard shortcut section below.)
* Minor internal bug fixes

v0.3.0 2015/09/14

* Rename to Astra SDK.
* Rename Sensor to StreamSet in C++ API.
* Various bug fixes and internal enhancements.
* New samples:  SimpleStreamViewer-SFML, SimpleColorViewer-SFML, ColorReaderEvent, ColorReaderPoll
* Samples have improved performance.
* Add IR stream, mirrored depth, and registered depth support.
* VS2013 samples solution no longer requires copying files - compiles and runs out of the box.
* StreamReader start() and stop() are functional now. See SimpleStreamViewer-SFML.
* Add initial getting started documentation

v0.2.1 2015/07/06 Updated Android and Windows drivers for new sensor USB IDs. Add Android test app .apk.

v0.2.0 2015/07/03 First version ready for external use.

Pre-built samples
=================

Pre-built samples are included in the bin/ directory.
Simply plug in your sensor and then run any of the executable files in the bin/ directory.

We recommend starting with SimpleStreamViewer-SFML and SimpleHandViewer-SFML.
In the hand viewer, wave left and right at the sensor a few times to start hand tracking.

OS-specific instructions:

OS X:
=======
Requires:
* OS X 10.8+
* Xcode 6.2+

Windows:
=======
If you want to run the pre-compiled samples and don't have Visual Studio installed,
you must install the Visual C++ Redistributable Packages.
For VS2013: https://www.microsoft.com/en-us/download/details.aspx?id=40784
For VS2015: https://www.microsoft.com/en-us/download/details.aspx?id=48145

Sample keyboard shortcuts
=========================

SimpleStreamViewer-SFML:

* F - toggle fullscreen
* R - toggle registered depth
* M - toggle mirrored streams
* I - enable IR (RGB mode)
* G - enable IR (Gray16 mode)
* C - enable color
* P - toggle pausing the streams
* O - toggle overlay color stream on depth stream

SimpleDepthViewer-SFML:

* F - toggle fullscreen
* R - toggle registered depth
* M - toggle mirrored streams
* P - toggle pausing the streams
* Space bar - toggle text overlay with the depth data under the mouse cursor

Building the samples from source
================================

OS X
====

The sample build system uses CMake 3.2+.

Getting CMake
=============

Option 1:
You can download it from https://cmake.org/download/

Option 2:
If you are a homebrew user (https://brew.sh), you can install the latest
CMake version by running the following in a terminal:
$ brew install cmake

Compiling Samples
=================

Once CMake has been properly installed, compiling the included samples
can be accomplished by enter the following at a terminal prompt:

$ cd /path/to/sdk
$ cd samples
$ ./build_samples.sh

> The samples will be compiled into samples/build/bin/

For additional sample build options e.g. creating an Xcode project or compiling in debug configuration:
$ ./build_samples.sh -h

Windows
=======

Requirements:
* Visual Studio 2013 or later. The VS 2015 Community version is a free download:
https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx
* Windows 7 or later. Tested on Windows 10, Windows 8.1, Windows 7.

The provided Visual Studio 2013/2015 solution is already configured to run out of the box.

Steps:
1) Open astra-samples.sln found either in samples/vs2013 or samples/vs2015 folder.
2) Once in Visual Studio, change the Solution Configuration to Release.
3) Build the solution.

> The samples will be compiled into samples/{vs{2013,2015}/bin/{Release,RelWithDebInfo,Debug}
  depending on your version of Visual Studio and the chosen build configuration.

Tips
====

You can exit samples by pressing Control-C. They will catch this signal and exit cleanly.
To exit samples with a GUI window, press Control-C, escape, or simply close the window.

When you start development, we highly recommend using the C++ API (or a higher level language wrapper.)
The C API is better suited for binding to other languages, or in environments where C is preferred.

Documentation
=============

Preliminary documentation in HTML format can be found in the sdk/docs directory.

Known Issues
============

* There is no error message if no sensor is found or plugged in. (But, it doesn't crash!)
  Hot-plugging (plugging in or removing the sensor while an app is running) is not currently supported.

* If a sample crashes or you forcibly terminate an app that uses Astra SDK before it is given a chance to
  properly shutdown the Astra sensor, the sensor driver may exhibit strange behavior.
  To FIX, simply unplug and re-plug the sensor into the USB port. (This is primarily an OpenNI issue.)

* If any samples appear to be running slow on your computer, make sure you are building the samples in
  Release configuration. This is the default if you are using the OSX build_samples.sh script.

* In Unity 5.5.4+ the body lags the color in MaskedColorStream

* Documentation is limited. We are working on full API documentation for a later release.

* Bone orientation/rotation can be inconsistent if passing through ambiguous positions, such as
  rotating the arm while it is straight out with the elbow unbent.

* On Android, body tracking performance is being improved. With all streams running, our
  Unity sample gets roughly 17 FPS on Orbbec Persee. With just the body stream, the Unity
  sample gets 30 FPS on Orbbec Persee.

* There are some APIs that are missing: camera hardware configuration (white balance, exposure, etc.),
  controlling the IR projector and Astra Mini IR flood, etc. These are coming soon.
