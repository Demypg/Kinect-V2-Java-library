using Microsoft.Kinect;
using System.Collections.Generic;
using System.Linq;

namespace KinectGestureLibrary
{
    /// <summary>
    /// The Raw data handler objects subscribe to any raw data events from the kinect and process it.
    /// The main purpose of this object is to recieve body data of an person in view and process the skeleton joints.
    /// 
    /// TODO:
    /// - Process other kinect data like: IR data, Depth Data, RGB Data
    /// </summary>
    class RawDataHandler
    {
        /// <summary> a reference to a common eventhandler </summary>
        private LibEventHandler evHandler;

        /// <summary> a reference to a kinectSensor </summary>
        private KinectSensor kinect;

        /// <summary> Reader for body frames </summary>
        private BodyFrameReader bodyFrameReader = null;

        /// <summary> Array for the bodies (Kinect will track up to 6 people simultaneously) </summary>
        private Body[] bodies = null;

        /// <summary> returns all trackingIds of the currently tracked bodies </summary>
        public ulong[] TrackingIDs
        {
            get
            {
                int maxBodies = this.kinect.BodyFrameSource.BodyCount;
                ulong[] list = new ulong[maxBodies];
                if (bodies != null)
                {
                    for (int i = 0; i < maxBodies; i++)
                    {
                        list[i] = bodies[i].TrackingId;
                    }
                }
                return list;
            }
        }

        /// <summary> List of gesture detectors, there will be one detector created for each potential body (max of 6) </summary>
        private List<GestureDetector> gestureDetectorList = null;

        /// <summary>
        /// A constructor for RawDataHandler, requires a reference to a kinect camera and a list of the active gestureDetectors.
        /// The reference to the gestureDetectors is solely stored because body data decides if a person is still in view,
        /// if a person has left the view, RawDataHandler will update the gestureDetector trackingID's and pause them.
        /// </summary>
        /// <param name="aKinect">A reference to the acitve Kinect camera</param>
        /// <param name="aEvHandler">An eventhandler so instances can send error logs</param>
        /// <param name="aGestDectList">A list of all active gestureDetectors which can be paused</param>
        public RawDataHandler(KinectSensor aKinect, LibEventHandler aEvHandler, List<GestureDetector> aGestDectList)
        {
            //store a reference to the common eventhandler
            this.evHandler = aEvHandler;

            //store a reference to the kinectSensor that is currently in use
            this.kinect = aKinect;

            //store a reference to the total list of gesturedetectors that have been made
            this.gestureDetectorList = aGestDectList;

            // open the reader for the body frames
            this.bodyFrameReader = this.kinect.BodyFrameSource.OpenReader();

            // set the BodyFramedArrived event notifier
            this.bodyFrameReader.FrameArrived += this.Reader_BodyFrameArrived;
        }

        /// <summary>
        /// Handles the body frame data arriving from the sensor and updates the associated gesture detector object for each body
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_BodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        // creates an array of 6 bodies, which is the max number of bodies that Kinect can track simultaneously
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                // we may have lost/acquired bodies, so update the corresponding gesture detectors
                if (this.bodies != null)
                {
                    // loop through all bodies to see if any of the gesture detectors need to be updated
                    int maxBodies = this.kinect.BodyFrameSource.BodyCount;
                    for (int i = 0; i < maxBodies; ++i)
                    {
                        bool bodiesChanged = false;
                        Body body = this.bodies[i];
                        ulong trackingId = body.TrackingId;
                        // if the current body TrackingId changed, update the corresponding gesture detector with the new value
                        if (trackingId != this.gestureDetectorList[i].TrackingId)
                        {
                            bodiesChanged = true;
                            this.gestureDetectorList[i].TrackingId = trackingId;

                            // if the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
                            // if the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                            this.gestureDetectorList[i].IsPaused = trackingId == 0;
                        }
                        if (bodiesChanged)
                        {
                            //convert the all ulong trackingID's to long trackingID's
                            //This is done to prevent problems with jni4net proxy generation. 
                            //Jni4net appearantly doesn't like ulong[] in event signatures.
                            long[] convertedList = new long[TrackingIDs.Count()];
                            for(int j = 0; j < TrackingIDs.Count(); j++)
                            {
                                convertedList[j] = (long)TrackingIDs[j];
                            }
                            evHandler.raiseTrackingIDsChangedEvent(convertedList);
                        }
                    }
                    //Raises an event to update the body view object
                    evHandler.raiseUpdateBodyEvent(bodies);
                }
            }
        }


    }
}
