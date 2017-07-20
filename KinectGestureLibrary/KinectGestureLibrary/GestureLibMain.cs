using System.Collections.Generic;
using Microsoft.Kinect;
using System.Threading;

namespace KinectGestureLibrary
{
    /// <summary>
    /// This is the main acces point of the library, Use this to initialise and start every function in the library.
    /// </summary>
    public class GestureLibMain
    {
        /// <summary> Active Kinect sensor </summary>
        private KinectSensor kinectSensor = null;

        /// <summary> Eventhandler for GestureLibrary </summary>
        public LibInterfaceEventHandler interfaceEventHandler { get; }

        /// <summary> Eventhandler for GestureLibrary </summary>
        public LibEventHandler eventHandler { get; }

        /// <summary>deserializer to deserialize any configuration files </summary>
        private ConfigSerializer configSerializer { get; }

        /// <summary> CoordinateMapper object </summary>
        public CoordinateMapper CoordinateMapper { get; set; }

        /// <summary> List of gesture detectors, there will be one detector created for each potential body (max of 6) </summary>
        private List<GestureDetector> gestureDetectorList = null;

        /// <summary> A common list to represent the database.</summary>
        private GestureHandler gestureHandler = null;

        /// <summary> Set true if you want the interface eventhandler to work</summary>
        public bool interfaceEventsMode { get; set; } = false;

        /// <summary>RawDataHandler object reference</summary>
        private RawDataHandler rawDataHandler;

        /// <summary> ManualResetEvent object </summary>
        private ManualResetEvent waitHandle;

        /// <summary>
        /// Constructor of the gestureLibraryMain class
        /// </summary>
        public GestureLibMain()
        {
            // Create a new waitHandler, this will hault the Thread in startKinect()
            this.waitHandle = new ManualResetEvent(false);

            // Create a new interface event handler , the interface event handler is made here so that you can get it directly from an instance of the libmain class
            this.interfaceEventHandler = new LibInterfaceEventHandler();

            // Create new event handler for the GestureLibrary
            this.eventHandler = new LibEventHandler(interfaceEventHandler);

            // initialize the gesture detection objects for our gestures
            this.gestureDetectorList = new List<GestureDetector>();

            //initialize the config serializer
            this.configSerializer = new ConfigSerializer(eventHandler);
        }

        /// <summary>
        /// Starts the kinect but ignores any config file
        /// </summary>
        /// <param name="aDbLocation">location of gesture database</param>
        public void startKinect(string aDbLocation)
        {
            // set the database location in gestureHandler
            this.gestureHandler = new GestureHandler(aDbLocation,eventHandler);
            // setup the kinect
            kinectStartup();
        }

        /// <summary>
        /// Starts the kinect and reads from a config file to setup gestures
        /// </summary>
        /// <param name="aDbLocation">location of a gesture database</param>
        /// <param name="aConfigLocation">a location for a config file</param>
        public void startKinect(string aDbLocation,string aConfigLocation)
        {
            // set the database location in gestureHandler
            this.gestureHandler = new GestureHandler(aDbLocation, eventHandler, configSerializer.deSerializeConfig(aConfigLocation));
            // setup the kinect
            kinectStartup();
        }

        /// <summary>
        /// Setup and start of the kinect
        /// </summary>
        public void kinectStartup()
        {
            // only one sensor is currently supported
            this.kinectSensor = KinectSensor.GetDefault();

            // open the sensor
            this.kinectSensor.Open();

            // create a gesture detector for each body (6 bodies => 6 detectors)
            int maxBodies = this.kinectSensor.BodyFrameSource.BodyCount;
            for (int i = 0; i < maxBodies; ++i)
            {
                GestureDetector detector = new GestureDetector(this.kinectSensor, eventHandler, gestureHandler);
                this.gestureDetectorList.Add(detector);
            }

            // create an instance of rawdatahandler that will fire events whenever raw data from the kinect is available
            this.rawDataHandler = new RawDataHandler(this.kinectSensor, eventHandler, gestureDetectorList);

            // Store a reference to the CoordinateMapper from kinect sensor
            this.CoordinateMapper = kinectSensor.CoordinateMapper;
            
            //when you utilize interface events, you have to halt this thread and let it wait for events
            //Any thread that calls startKinect() while interfaceEventsMode is true will get haltet and will infinitely wait for events.
            if (interfaceEventsMode)
            {
                waitHandle.WaitOne();
            }
        }

        /// <summary>
        /// Creates a example config in the root folder of this project
        /// </summary>
        public void createExampleConfig()
        {
            new ConfigExampleCreator(@"configExample.xml", configSerializer);
        }

        
    }
}
