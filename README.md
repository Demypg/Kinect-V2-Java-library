# Kinect-V2-Java-library
A universal library that could be used in both C# and Java to connect to a Microsoft Kinect v2.
Not to be confused with the first generation kinect that was rereleased for Windows computers, this project uses the newest Kinect released in 2013, also known as [Kinect for XBOX one](http://www.xbox.com/en-US/xbox-one/accessories/kinect). 

This library was a result of a bachelor thesis conducted at NTNU Ålesund where the requirements were to make a library that could be loaded in Java and handle gesture-data from a Kinect v2. Current features of the library are:

- Fully setup connection with a Kinect v2
- Load a gesture database that has been build with Visual Gesture Builder (.gbd files)
- Interface event system for Java software
- Delegate event system for C# software
- An external configuration file for setup of gestures from the gesture database.
- Link gestures together where the resulting combined gesture has the confidence of the gesture in the link list with highest confidence.
- A crude Average-filter for confidence values

## Getting Started

Clone the repo and build the solution "KinectGestureLibrary" in Visual Studio. More in the section "installing".

### Prerequisites

- Kinect V2, setup at [KinectV2 Setup](http://support.xbox.com/en-US/xbox-on-windows/accessories/kinect-for-windows-v2-setup)
- [Kinect SDK 2.0](https://www.microsoft.com/en-us/download/details.aspx?id=44561)
- Visual studio IDE 2015 or newer at [Visual studio](https://www.visualstudio.com/)
- JNI4net, can be found at [JNI4net](https://github.com/jni4net/jni4net)
- [Netbeans IDE](https://netbeans.org/) (or any other Java IDE of your choice) 

### Installing

Prerequired installations:
Be sure to have Kinect SDK 2.0 installed on your Windows machine. 
1. Download the entire repo and place it in a recognizable folder.
2. If you are going to use this library with Java, download the JNI4net binaries and place the "build" and "lib" folder in the /JNI4NET/Build_library

Library setup for use in C#:
1. Load the solution "KinectGestureLibrary" in Visual studio and add these references:
- Microsoft.Kinect
- Microsoft.Kinect.VisualGestureBuilder (x64 version)
2. Build solution
3. Copy the "vgbtechs" folder from "C:\Program Files\Microsoft SDKs\Kinect\v2.0_1409\Tools\KinectStudio" (location might vary, but its located in the folder where Kinect Studio got installed after the Kinect SDK was installed) to the folder where the build solution is located (e.g. Debug folder).
4. Copy the example config from the /Configurations folder in your build folder and change the settings so it fits with your gesture database.
5. Done! You can now include the library in your C# solution.

Example on how to include the library in C#:
```
// Start main class of the GestureLibrary
this.lib = new GestureLibMain();
//start the kinect()
this.lib.startKinect(@"Sitting.gbd", @"configExample.xml");
//Or without a config file:
//this.lib.startKinect(@"Sitting.gbd");

//Get the event handler from the library
this.EventHandler = lib.eventHandler;
//Get the CoordinateMapper from the Kinect from the GestureLibrary if needed to calculate 3D 
//this.CoordinateMapper = lib.CoordinateMapper;
 ```

Library setup for use in Java:
1. Continue from step 4. from the C# setup.
2. Copy all the contents of your build folder to the "build" folder located in /JNI4NET/Build_library.
3. Run the create.bat file located in /JNI4NET/Build_library with developer command prompt from Visual Studio
4. Done! The Java library has been generated and placed in the /JNI4NET/Build_library/final_result. Read further on the JNI4net wiki on how to implement this type of library or check out the example Java project in the /Java folder.

Configuration setup:


## FAQ 
Q: Will this repo be updated by the authors?
A: Most likely not, the project was uploaded so that other people could take advantadge of premade code for the kinect V2.

Q: Can I contact you about further questions?
A: You can contact me at [Demy Gielesen](mailto:demy.gielesen@outlook.com)

Q: Are you going to add more questions to this list?
A: If I come up with more, yes.

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Authors

* **Demy Gielesen** - *Lead coder of library*
* **Malén Barstad** - *Lead coder of GUI and testing databases(not included in this repo)* 

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Our supervisors who guided us troughout the Bachelor course
* Hat tip to anyone who's code was used
* Inspiration
