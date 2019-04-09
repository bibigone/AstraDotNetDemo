Astra SDK v2.0.15
Copyright (c) 2015-2019 Orbbec
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
* Android 6.0, Android 7.0, Android 8.1
* Unity3D 2017, Unity3D 2018, Unity 4.7

Orbbec Body Tracking trial time expiration
===============
For this release, Orbbec Body Tracking expires on 2019/06/30
and will stop operating. Please make sure to update to a new version before then.
You can now set your license string to extend the trial. See API notes below for
orbbec_body_tracking_set_license().

For support on the trial expiration or to extend your trial, please contact
info@orbbec3d.com.

What's New
==========

v2.0.15 2019/03/15
* Add support for the Orbbec Astra Deeyea camera model and Orbbec Astra DaBai camera model.
* Add device opened notification callbacks on Android for Unity. You can implement this
  interface and use it when calling "openAllDevices" on "AstraUnityPlayerActivity",
  For more information, see AstraUnityContext.cs in Unity Sample.
  C#:
    public class AstraDeviceManagerListener : AndroidJavaProxy
    {
        public AstraDeviceManagerListener() : 
            base("com.orbbec.astra.android.AstraDeviceManagerListener") {}

        //When open completed will call this. In this function, before you create streams,
        //you should check the number of available devices by calling "getAvailableDevicesSize"
        //on "AstraUnityPlayerActivity". If number is zero, Don't create streams.
        void onOpenAllDevicesCompleted(AndroidJavaObject availableDevices) {}

        //Can't work currently.
        void onOpenDeviceCompleted(AndroidJavaObject device, AndroidJavaObject opened) {}

        //There is no Orbbec device to open.
        void onNoDevice() {}

        //Requesting usb permission is denied by user.
        void onPermissionDenied(AndroidJavaObject device){}
    }
* Support getting NV21 format image from ColorStream on Android when using Astra Pro and
  Astra Pro Plus.
  Java: ColorStream stream = ColorStream.getNV21ColorStream(streamReader);
        ColorFrame colorFrame = ColorFrame.getNV21ColorFrame(readerFrame);
  C#:   using Astra.Core;
        ColorStream stream = streamReader.GetStream<ColorStream>(StreamSubType.COLOR_NV21_SUBTYPE);
        ColorFrame colorFrame = frame.GetFrame<ColorFrame>(StreamSubType.COLOR_NV21_SUBTYPE);
* Add D (for Deeyea and DaBai) keyboard shortcut to SimpleDepthViewer-SFML, SimpleBodyViewer-SFML
  and MaskedColorViewer-SFML to work with Deeyea and DaBai.
* Add N keyboard shortcut to MaskedColorViewer-SFML to mirror color stream.
* Support skeleton recognition for five people.


v2.0.14 2018/12/10
* Add device opened notification callbacks on Android
* Improve body joint alignment with depth image (fix issue related to field-of-view)

v2.0.13 2018/11/22
* Add Skeleton Optimization level APIs. The optimization level gives the developer
  control over the balance between memory usage and accuracy for skeleton tracking.
  There are 9 optimization levels, with 1 being the minimum memory but worst tracking
  accuracy, and 9 requiring the most memory but best tracking accuracy. Level 9 is
  the default. Level 2 is the minimum recommended level. Optimization enums have suggested
  values: Min Memory = 2, Balanced = 5, Best Accuracy = 9.
  C:    astra_skeleton_optimization_t optimization;
        astra_bodystream_get_skeleton_optimization(bodyStream, &optimization);
        astra_bodystream_set_skeleton_optimization(bodyStream, optimization);
  C++:  SkeletonOptimization optimization = bodyStream.get_skeleton_optimization();
        bodyStream.set_skeleton_optimization(optimization);
  C#:   SkeletonProfile optimization = bodyStream.GetSkeletonProfile();
        bodyStream.SetSkeletonProfile(optimization);
  Java: SkeletonOptimization optimization = bodyStream.getSkeletonOptimization();
        bodyStream.setSkeletonOptimization(optimization);
* Unity sample has been updated with UI controls for Skeleton Optimization
* SkeletonProfile.UpperBody is now available. This profile includes all joints
  above MidSpine. Unity sample has been updated with UI option to select Upper Body Profile.
* POTENTIALLY BREAKING: The Java wrapper has been updated so that various classes
  that were marked public are now internal.
* Add support for the Orbbec Astra Pro Plus camera model.
* Docs now contains more docs.
* C# wrapper now allocates even less garbage-collected memory per frame.
* Add "Debug Text" toggle option in Unity wrapper sample application. This is enabled
  by default. When disabled, the FPS display and internal timings are cleared.
  This reduces the GC memory allocation by approximately 2k per frame, useful for profiling.
* Fix stream corruption issue in Unity wrapper sample application when switching scenes.
* Fix issue where depth stream may be stopped when the color stream was stopped,
  and vice versa.
* Fix issue where MaskedColorStream and ColorizedBodyStream both stopped when one
  of those streams was stopped.
* Fix issue where terminating and then initializing the SDK on Android didn't work.
* Frame skipping is now disabled in Orbbec Body Tracking. We expect that
  previous improvements to the Unity wrapper allowing asynchronous processing
  will result in a better overall experience.
* Minor internal bug fixes
* Internal code cleanup

v2.0.12 2018/09/30
* In C++ API, RGBAPixel was renamed to RgbaPixel for style consistency.
  There is a typedef RgbaPixel RGBAPixel; so existing code still works,
  but RGBAPixel is deprecated.
* Adds C++-style serial number API to C++ API:
  C++:    DepthStream depthStream = [...];
          std::string serialNumber = depthStream.serial_number();
  The existing C-style method in the DepthStream class is still available
  for compatibility reasons but is considered deprecated:
          void serial_number(char* serialnumber, uint32_t length) const;
* Add astra_reader_has_new_frame() thread-safe C API and implementation.

  This method is designed to be safe to call from any thread. Internally,
  it stores the status in an atomic_bool, which is thread-safe.
  The reason for making this thread-safe is so that astra_update() can
  be called in a loop in a background thread, and the main thread can
  check whether it should try to sync with the background thread so that
  it can open a new frame from readers.

  This is a short-term solution that helps support a specific threading
  scenario until the full Astra API is updated to be thread-safe. In this
  threading scenario, the app must guarantee that the Astra API is still
  only called from one thread at a time through its own synchronization
  (mutex, etc.), with the one exception that astra_reader_has_new_frame()
  may be called from any thread.
  C:    astra_reader_t reader = [...];
        bool hasNewFrame;
        reader_has_new_frame(reader, &hasNewFrame)
  C++:  astra::StreamReader reader = [...];
        bool hasNewFrame = reader.has_new_frame();
  C#:   Astra.StreamReader reader = [...];
        bool hasNewFrame = reader.HasNewFrame();
  Java: Astra.StreamReader reader = [...];
        boolean hasNewFrame = reader.hasNewFrame();
* Add P (for Profile) keyboard shortcut to SimpleBodyViewer-SFML to toggle
  between Full skeleton profile and Basic skeleton profile.
* Add T (for Tracking) keyboard shortcut to SimpleBodyViewer-SFML to toggle
  between Segmentation, Segmentation+Body, and Segmentation+Body+HandPoses
  as default body feature tracking states.
* Performance of MaskedColorStream processing has been improved significantly
  (roughly 4x faster depending upon platform and image ratios) through the use
  of cross-platform SIMD code.
* Fix issue in Java wrapper where MaskedColorStream/ColorizedBodyStream used
  ImageStream as base class. Those now derive from DataStream as intended.
* Fix issue in Java wrapper where PointFrame.getPointBuffer() called itself
  recursively instead of returning the point buffer. It now returns the point buffer.
* Fix issue in Java wrapper where PointStream.get() wasn't public. It is now public.
* Fix inconsistent definition/declaration of astra_core_version
* Fix issue in OBT where skeletons would be tracked even in Segmentation-only mode
* Fix issue in OBT where if a user moves further than four meters from the sensor
  (the max for skeleton tracking) the user would not get skeleton tracking again
  until they left the frame completely. Now the user can walk further than four meters
  then closer and will get skeleton tracking again.
* Fix issue in OBT where a class was inappropriately being used by multiple threads
  and caused some rare crashes on certain Android machines. Now each thread has its
  own instance of the class.
* Fix issue in OBT where changing the SkeletonProfile would not re-initialize a
  type properly. The type now reinitialized property.
* Fix issue in OBT where the legs was not allowed to rotate more than a
  certain angle. The legs can now be tracked with larger leg swings.
* Streams now used aligned memory allocation for all stream bins. This is to support
  SIMD operations on bin data.
* Add ASTRA_ALIGN(N) macro to headers to explcitly align certain fields as necessary.
* Update CMakeLists so Astra SDK can be built with Visual Studio 2017 (compiler 19.14).
* Refactor software-registration code in MaskedColorStream for legacy Astra mx400 cameras
* Internal changes to protect against some crashes in certain multi-threading
  scenarios. With the exception of astra_reader_has_new_frame(), the API
  must still be called from only a single thread. Deadlocks and crashes are
  still possible if multiple threads access the API simultaneously.
* Minor changes to naming and refactoring to sample code to clarify the purpose of the code
* Internal code cleanup


v2.0.11-beta5 2018/08/01
* BREAKING: In Java, Body.getId() now returns an int instead of a byte.
  This fixes an issue where body ids would appear to be negative when the id
  is greater than 127.
* Add SkeletonProfile API. This allows the developer to select from several options
  for which skeleton joints are tracked. Full profile tracks all available joints
  and basic profile only tracks head, MidSpine, left hand, and right hand.
  Basic profile is more accurate for the tracked joints and takes less time to track.
  C:    astra_skeleton_profile_t profile;
        astra_bodystream_get_skeleton_profile(bodyStream, &profile);
        astra_bodystream_set_skeleton_profile(bodyStream, profile);
        Valid options for astra_skeleton_profile_t include:
        ASTRA_SKELETON_PROFILE_FULL
        ASTRA_SKELETON_PROFILE_BASIC
  C++:  bodyStream.set_skeleton_profile(SkeletonProfile::Full); // or SkeletonProfile::Basic
        SkeletonProfile profile = bodyStream.get_skeleton_profile();
  C#:   bodyStream.SetSkeletonProfile(SkeletonProfile.Full); // or SkeletonProfile.Basic
        SkeletonProfile profile = bodyStream.GetSkeletonProfile();
  Java: bodyStream.setSkeletonProfile(SkeletonProfile.Full); // or SkeletonProfile.Basic
        SkeletonProfile profile = bodyStream.getSkeletonProfile();
* Improve the C# wrapper to significantly reduce heap allocation.
* Optimized Android yuv2rgb conversion, saving up to 2.5ms on low-end Android boxes
* Fix issue where the body center of mass could be NaN if certain joints were not tracked
* Fix issue where OpenNI2 could crash if the driver dlls are in a non-standard directory
* Fix issue where a zero byte astra.log is created even if file_output is false in astra.toml.
  Now astra.log is no longer created when file_output is false.
* Fix issue where some plugins were requesting the device serial number with too small
  of a string. 256 bytes is recommended the serial number char[] size.
* Fix out-of-bounds memory access in Orbbec Body Tracking
* Fix issue where OBT licensing periodic check can fail after the trial expiration date,
  even with a valid license
* Fix issue in OBT where the position of the head joint was inappropriately influenced
  by other joints
* Fix bug where if the first body frame is estimated (due to a very slow CPU) then
  the body frame would have garbage data. The data is now properly initialized.
* Fix issue where skeletonizer could have been deleted twice
* Refactor data loading methods in OBT to reduce memory overhead and slightly
  reduce startup time.
* Internal improvements to performance profiling and fixing some minor compiler warnings.


v2.0.10-beta4 2018/07/05

* BREAKING: In C++, the CoordinateMapper class's methods convert_depth_to_world()
  and convert_world_to_depth() now take the output parameters by reference instead of by pointer:
  void convert_depth_to_world(float  depthX, float  depthY, float  depthZ,
                              float& worldX, float& worldY, float& worldZ) const;
  void convert_world_to_depth(float  worldX, float  worldY, float  worldZ,
                              float& depthX, float& depthY, float& depthZ) const;
* Add stream availability API. This allows developers to determine if a stream will
  ever return data. A stream may not be available if the sensor is not plugged in or
  if the sensor or plugin does not support that stream. This may depend upon the current
  sensor configuration. A stream that is not available will not receive start()/stop() commands
  and will not process get/set/invoke commands such as setting the resolution.
  Once the stream becomes available, you must call start() and configure the resolution.
  C:    astra_depthstream_is_available(astra_depthstream_t depthStream, bool* isAvailable)
        and similar for every other stream
  C++:  astra::DepthStream depthStream = reader.stream<astra::DepthStream>();
        bool isAvailable = depthStream.is_available()
        and similar for every other stream
  C#:   DepthStream depthStream = reader.GetStream<DepthStream>();
        bool isAvailable = depthStream.IsAvailable;
        and similar for every other stream
  Java: DepthStream depthStream = DepthStream.get(reader);
        boolean isAvailable = depthStream.getIsAvailable();
* Add StreamSet availability API. This allows developers to determine if a StreamSet is available.
  A StreamSet is available when a sensor is plugged in that matches the StreamSet URI such as device/sensor0.
  If a sensor is unplugged or was not yet plugged in, the StreamSet is not available.
  Any streams accessed from an unavailable StreamSet will also be unavailable.
  C:    astra_streamsetconnection_t streamSet;
        astra_streamset_open("device/sensor1", &streamSet);
        astra_streamset_is_available(streamSet, bool* isAvailable);
  C++:  StreamSet streamSet("device/sensor1");
        bool isAvailable = streamSet.is_available();
  C#:   StreamSet streamSet = StreamSet.Open("device/sensor1");
        bool isAvailable = streamSet.IsAvailable;
  Java: StreamSet streamSet = StreamSet.open("device/sensor1");
        boolean isAvailable = streamSet.getIsAvailable();
* Add StreamSet URI API. This allows developers to determine the URI of a StreamSet.
  Typically this is the URI that is passed in when opening the StreamSet, but if the
  StreamSet was default constructed or used "device/default", then the URI will be
  "device/sensor0".
  C:    char uri[ASTRA_STREAMSET_URI_MAX_LENGTH];
        astra_streamsetconnection_t streamSet;
        astra_streamset_open("device/default", &streamSet);
        astra_streamset_get_uri(streamSet, uri, ASTRA_STREAMSET_URI_MAX_LENGTH);
        // uri now contains "device/sensor0"
        The method copies the URI into the char* memory provided by the caller.
  C++:  StreamSet streamSet;
        std::string uri = streamSet.uri(); //"device/sensor0"
  C#:   StreamSet streamSet = StreamSet.Open();
        string uri = streamSet.Uri; //"device/sensor0"
  Java: StreamSet streamSet = StreamSet.open();
        String uri = streamSet.getUri();
* Add IsEstimated API for BodyFrame in C# and Java wrappers. This was previously
  added to BodyFrameInfo, but in those wrappers BodyFrameInfo is not accessible.
  This fixes the issue and publicly exposes IsEstimated.
  C#:   bool isEstimated = bodyFrame.IsEstimated;
  Java: boolean isEstimated = bodyFrame.getIsEstimated();
* DepthStream field-of-view now changes when registration is enabled. When registration
  is enabled, the DepthStream field-of-view matches the ColorStream.
* Update body tracking to use correct field-of-view as provided by the DepthStream
* Optimized memory usage in Orbbec Body Tracker to reduce overall RAM usage by about 50 MB.
  Accessing less memory also leads to a small performance improvement.
* Fix issue where bodies that went beyond the max skeleton range of 4 meters would never
  get their skeletons back when they come back within 4 meters.
* Fix issue where sometimes a UVC color device (e.g. Astra Pro color) fails to open.
  The UVC device may fail to open if the device is not ready yet. Now the SDK will
  attempt to open the UVC device repeatedly with a short delay between each attempt.
* Fixes a crash on Linux when more than one UVC color device (Astra Pro) is plugged in.
  The SDK still only supports one UVC color devices at a time, but now additional UVC
  color devices will not cause a crash and you can use the non-color streams from
  the additional sensors.
* Fix issue where the stream bin system would allocate more memory than necessary.
  This reduces the memory usage of active streams.
* Fix issue in MaskedColorStream when the resolution of the color stream is smaller
  than the resolution of the body stream/depth stream.
* Fix issue in SimpleBodyViewer-SFML where the Neck and Wrist joints were not properly
  connected to the skeleton visualization
* Fix Java JNI bug where getting a boolean property (such as getMirroring(), getRegistration())
  may not have returned the correct value.
* Fix issue where an internal stage of the hand pose recognizer selected an incorrect value
  from a list of points on the user's hand.
* Fix issue where an internal stage of the hand pose recognizer may have dereferenced invalid memory
* Fix issue in body tracking where an iterator was used incorrectly when erasing lost bodies
* Fix various minor logic issues found when auditing the Orbbec Body Tracking segmentation module
* Various internal code cleanup and minor changes to support new features
* Orbbec Body Tracking trial expiration set to September 30, 2018

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

* There are some APIs that are missing: camera hardware configuration (white balance, exposure, etc.),
  controlling the IR projector and Astra Mini IR flood, etc. These are coming soon.
