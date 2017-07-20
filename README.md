# Kinect-V2-Java-library
A universal library that could be used in both C# and Java to connect to a Microsoft Kinect v2.
Not to be confused with the first generation kinect that was rereleased for Windows computers, this project uses the newest Kinect released in 2013, also known as [Kinect for XBOX one](http://www.xbox.com/en-US/xbox-one/accessories/kinect). 

This library was a result of a bachelor thesis conducted at NTNU Ålesund where the requirements were to make a library that could be loaded in Java and handle gesture-data from a Kinect v2. Current features of the library are:

- Fully setup connection with a Kinect v2
- Load a gesture database that has been build with Visual Gesture Builder (.gbd files)
- Interface event system for Java software
- Delegate event system for C# software
- An external configuration file for setup of gestures from the gesture database.

## Getting Started

Clone the repo and build the project "kinectLibrary" in Visual Studio. More in the section "installing".

### Prerequisites

- Kinect V2, setup at [KinectV2 Setup](http://support.xbox.com/en-US/xbox-on-windows/accessories/kinect-for-windows-v2-setup)
- [Kinect SDK 2.0](https://www.microsoft.com/en-us/download/details.aspx?id=44561)
- Visual studio IDE 2015 or newer at [Visual studio](https://www.visualstudio.com/)
- JNI4net, can be found at [JNI4net](https://github.com/jni4net/jni4net)
- [Netbeans IDE](https://netbeans.org/) (or any other Java IDE of your choice) 


### Installing

A step by step series of examples that tell you have to get a development env running

Say what the step will be

```
Give the example
```

And repeat

```
until finished
```

End with an example of getting some data out of the system or using it for a little demo

## Running the tests

Explain how to run the automated tests for this system

### Break down into end to end tests

Explain what these tests test and why

```
Give an example
```

### And coding style tests

Explain what these tests test and why

```
Give an example
```

## Deployment

Add additional notes about how to deploy this on a live system

## Built With

* [Dropwizard](http://www.dropwizard.io/1.0.2/docs/) - The web framework used
* [Maven](https://maven.apache.org/) - Dependency Management
* [ROME](https://rometools.github.io/rome/) - Used to generate RSS Feeds

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.

## Authors

* **Demy Gielesen** - *Lead coder of library*
* **Malén Barstad** - *Lead coder of GUI and testing databases(not included in this repo)* 

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Hat tip to anyone who's code was used
* Inspiration
* etc
