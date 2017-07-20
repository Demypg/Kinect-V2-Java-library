namespace KinectGestureLibrary
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Kinect;
    using Microsoft.Kinect.VisualGestureBuilder;
    using System.Linq;

    /// <summary>
    /// Gesture Detector class which listens for VisualGestureBuilderFrame events from the service
    /// and calls various events such as TrackingId'sChangedEvent, UpdateBodyEvent and GestureResultEvent
    /// </summary>
    class GestureDetector : IDisposable
    {
        /// <summary> Path to the gesture database that was trained with VGB </summary>
        private readonly string dbLocation = @"C:\Gesture.gbd"; //placeholder name, is never used

        /// <summary> Gesture frame source which should be tied to a body tracking ID </summary>
        private VisualGestureBuilderFrameSource vgbFrameSource = null;

        /// <summary> Gesture frame reader which will handle gesture events coming from the sensor </summary>
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        /// <summary> Event handler where every event will be triggered from </summary>
        private LibEventHandler eventHandler = null;

        /// <summary> A gestureHandler to convert the list of gestures to a new list with added configuration</summary> 
        private GestureHandler gestureHandler = null;

        private int bufferSize = 10;
        private int bufferCount = 0;
        private IReadOnlyDictionary<string, float>[] discreteResultsBuffer;
        private IReadOnlyDictionary<string, float>[] continuousResultsBuffer;



        /// <summary>
        /// Initializes a new instance of the GestureDetector class along with the gesture frame source and reader
        /// </summary>
        /// <param name="kinectSensor">Active sensor to initialize the VisualGestureBuilderFrameSource object with</param>
        /// <param name="aEventHandler">The main event handler that accepts all event triggers</param>
        /// <param name="aGestureHandler">A gesture configurator object</param>
        public GestureDetector(KinectSensor kinectSensor, LibEventHandler aEventHandler, GestureHandler aGestureHandler)
        {
            //test
            if (kinectSensor == null)
            {
                throw new ArgumentNullException("kinectSensor");
            }
            
            //store the location of the database
            this.dbLocation = aGestureHandler.DatabaseLocation;

            //store a reference to the gestureHandler
            this.gestureHandler = aGestureHandler;

            // store a reference to a common event handler
            this.eventHandler = aEventHandler; 

            // create the vgb source. The associated body tracking ID will be set when a valid body frame arrives from the sensor.
            this.vgbFrameSource = new VisualGestureBuilderFrameSource(kinectSensor, 0);

            // open the reader for the vgb frames
            this.vgbFrameReader = this.vgbFrameSource.OpenReader();

            Console.WriteLine("VGB capture initialized on a body.");

            if (this.vgbFrameReader != null)
            {
                this.vgbFrameReader.IsPaused = true;
                this.vgbFrameReader.FrameArrived += this.Reader_GestureFrameArrived;
            }

            try
            {
                // load the gesture database
                using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(this.dbLocation))
                {

                    // load all gestures from the gesture database
                    try
                    {
                        this.vgbFrameSource.AddGestures(database.AvailableGestures);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Couldn't locate AdaboostTech.dll and/or RFRProgressTech.dll");
                        Console.WriteLine(e.StackTrace);
                        eventHandler.raiseMessageEvent(1, "Couldn't locate AdaboostTech.dll and/or RFRProgressTech.dll");
                        throw;
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine("Couldn't locate database file");
                Console.WriteLine(e.StackTrace);
                eventHandler.raiseMessageEvent(1, "Couldn't locate database file");
                throw;
            }

            bufferSize = gestureHandler.getBufferSize();
            discreteResultsBuffer = new IReadOnlyDictionary<string, float>[bufferSize];
            continuousResultsBuffer = new IReadOnlyDictionary<string, float>[bufferSize];

        }

        /// <summary>
        /// Gets or sets the body tracking ID associated with the current detector
        /// The tracking ID can change whenever a body comes in/out of scope
        /// </summary>
        public ulong TrackingId
        {
            get
            {
                return this.vgbFrameSource.TrackingId;
            }

            set
            {
                if (this.vgbFrameSource.TrackingId != value)
                {
                    this.vgbFrameSource.TrackingId = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the detector is currently paused
        /// If the body tracking ID associated with the detector is not valid, then the detector should be paused
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return this.vgbFrameReader.IsPaused;
            }

            set
            {
                if (this.vgbFrameReader.IsPaused != value)
                {
                    this.vgbFrameReader.IsPaused = value;
                }
            }
        }

        /// <summary>
        /// Disposes all unmanaged resources for the class
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader objects
        /// </summary>
        /// <param name="disposing">True if Dispose was called directly, false if the GC handles the disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.vgbFrameReader != null)
                {
                    this.vgbFrameReader.FrameArrived -= this.Reader_GestureFrameArrived;
                    this.vgbFrameReader.Dispose();
                    this.vgbFrameReader = null;
                }
            }
        }


       

        /// <summary>
        /// Handles gesture detection results arriving from the sensor for the associated body tracking Id
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            //check if this gesture detector was paused by its body(when a body leaves view, it gets paused)
            if (!IsPaused)
            {
                VisualGestureBuilderFrameReference frameReference = e.FrameReference;
                using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
                {
                    if (frame != null)
                    {
                        // get the discrete gesture results which arrived with the latest frame
                        IReadOnlyDictionary<string, float> discreteResults;
                        IReadOnlyDictionary<string, float> continuousResults;
                        
                        if(bufferCount < bufferSize)
                        {
                            IReadOnlyDictionary<string, float> tempDict = gestureHandler.getLinkedDiscreteResults(frame.DiscreteGestureResults);
                            if (tempDict != null)
                            {
                                discreteResultsBuffer[bufferCount] = tempDict;
                                continuousResultsBuffer[bufferCount] = gestureHandler.getLinkedContinuousResult(frame.ContinuousGestureResults);
                                bufferCount++;
                            }
                        }
                        else //if (discreteResults != null)//Check for dicrete results only, as a gesture always has some discrete data.
                        {
                            bufferCount = 0;
                            discreteResults = getAverageDiscreteResults();
                            continuousResults = getAverageContinuousResults();

                            Dictionary<string, float> confidenceReturnList = new Dictionary<string, float>();//multiple gestures can have the same confidence, use name as key
                            Dictionary<string, float> progressReturnList = new Dictionary<string, float>();//multiple gestures can have the same confidence, use name as key

                            List<PseudoGesture> linkedGestureList = gestureHandler.linkGestures(this.vgbFrameSource);
                            //iterate trough the entire list of possible (pseudo) gestures
                            foreach (PseudoGesture gesture in linkedGestureList)
                            {
                                if (gesture.gestureType == GestureType.Discrete)
                                {
                                    float DiscreteResult = discreteResults[gesture.gestureName];
                                    if (DiscreteResult != 0)
                                    {
                                        // update the GestureResultView object with new gesture result values
                                        this.eventHandler.raiseGestureResultEvent((long)this.TrackingId, gesture.gestureName, DiscreteResult);

                                        //we want a list that consists of max 3 gestures:

                                        if (confidenceReturnList.Count < 3)//add 3 entries before checking which is bigger
                                        {
                                            confidenceReturnList.Add(gesture.gestureName, DiscreteResult);
                                        }
                                        else
                                        {
                                            bool bigger = false;//set if this gesture has a higher confidence then any of the stored gestures
                                            foreach (KeyValuePair<string, float> entry in confidenceReturnList)
                                            {
                                                if (DiscreteResult <= entry.Value)
                                                {
                                                    bigger = true;
                                                }
                                            }
                                            if (bigger)
                                            {
                                                //add the gesture that has a bigger confidence level to the list
                                                confidenceReturnList.Add(gesture.gestureName, DiscreteResult);

                                                //but now the list has 4 entries instead of 3, the following code removes the entry with lowest confidence:

                                                float lowest = 0;//set to be the same as the gesture with lowest confidence
                                                foreach (KeyValuePair<string, float> entry in confidenceReturnList)
                                                {
                                                    if (entry.Value < lowest || lowest == 0)
                                                    {
                                                        lowest = entry.Value;
                                                    }
                                                }
                                                string keyToRemove = "";//find the gesture name of the gesture with the lowest confidence
                                                foreach (KeyValuePair<string, float> entry in confidenceReturnList)
                                                {
                                                    if (entry.Value == lowest)
                                                    {
                                                        keyToRemove = entry.Key;
                                                        //in case multiple gestures have the same confidence (many probably have 0 as confidence)
                                                        //only remove the first found entry that has the lowest confidence so break, making the list size back to 3
                                                        break;
                                                    }
                                                }
                                                if (!keyToRemove.Equals(""))//redundant, but check if we found the gesture that has the lowest confidence
                                                {
                                                    confidenceReturnList.Remove(keyToRemove);
                                                }
                                            }//END OF --- if(bigger)
                                        }//END OF --- if (confidenceList.Count < 3) else
                                    }//END OF --- if(Discreteresults != null)
                                }//END OF --- if (gesture.GestureType == GestureType.Discrete)

                                if (gesture.gestureType == GestureType.Continuous && continuousResults != null)
                                {
                                    float ContinuousResult = continuousResults[gesture.gestureName];
                                    if (ContinuousResult != 0)
                                    {
                                        // update the GestureResultView object with new gesture result values
                                        this.eventHandler.raiseGestureContinuousResultEvent((long)this.TrackingId, gesture.gestureName, ContinuousResult);

                                        //we want a list that consists of max 3 gestures:

                                        if (progressReturnList.Count < 3)//add 3 entries before checking which is bigger
                                        {
                                            progressReturnList.Add(gesture.gestureName, ContinuousResult);
                                        }
                                        else
                                        {
                                            bool bigger = false;//set if this gesture has a higher progress then any of the stored gestures
                                            foreach (KeyValuePair<string, float> entry in progressReturnList)
                                            {
                                                if (ContinuousResult <= entry.Value)
                                                {
                                                    bigger = true;
                                                }
                                            }
                                            if (bigger)
                                            {
                                                //add the gesture that has a bigger progress level to the list
                                                progressReturnList.Add(gesture.gestureName, ContinuousResult);

                                                //but now the list has 4 entries instead of 3, the following code removes the entry with lowest progress:

                                                float lowest = 0;//set to be the same as the gesture with lowest progress
                                                foreach (KeyValuePair<string, float> entry in progressReturnList)
                                                {
                                                    if (entry.Value < lowest || lowest == 0)
                                                    {
                                                        lowest = entry.Value;
                                                    }
                                                }
                                                string keyToRemove = "";//find the gesture name of the gesture with the lowest progress
                                                foreach (KeyValuePair<string, float> entry in progressReturnList)
                                                {
                                                    if (entry.Value == lowest)
                                                    {
                                                        keyToRemove = entry.Key;
                                                        //in case multiple gestures have the same progress (many probably have 0 as progress)
                                                        //only remove the first found entry that has the lowest confidence so break, making the list size back to 3
                                                        break;
                                                    }
                                                }
                                                if (!keyToRemove.Equals(""))//redundant, but check if we found the gesture that has the lowest progress
                                                {
                                                    progressReturnList.Remove(keyToRemove);
                                                }
                                            }//END OF --- if (bigger)
                                        }//END OF --- if (progressList.Count < 3) else
                                    }//END OF --- if(ContinuousResult != null)
                                }//END OF --- if(gesture.GestureType == GestureType.Continuous && continuousResults != null)
                            }//END OF --- Foreach(Gesture gesture in this.vgbFrameSource.Gestures)

                            //Check if the gestures in the list actually have some value
                            //No need to check for values in the progressList as discretegestures will always be fired first
                            bool fireGestureListEvent = false;
                            foreach (KeyValuePair<string, float> entry in confidenceReturnList)
                            {
                                if (entry.Value != 0)
                                {
                                    fireGestureListEvent = true;
                                }
                            }
                            if (fireGestureListEvent)//fire event when the gestures do have some value
                            {
                                this.eventHandler.raiseGestureListEvent(confidenceReturnList, progressReturnList, (long)TrackingId);
                            }

                        }//END OF ---  if(discreteResults != null)
                    }//END OF --- if(frame != null)
                }
            }
        }


        /// <summary>
        /// creates a new dictionairy that averages the discrete results in the buffer
        /// </summary>
        /// <returns>a dictionairy filled with averege gesture results</returns>
        private IReadOnlyDictionary<string, float> getAverageDiscreteResults()
        {
            //final result dictionairy
            Dictionary<string, float> returnDict = new Dictionary<string, float>();
            if (discreteResultsBuffer.Count() > 1)
            {
                //the final value after the average has been calculated
                float finalValue = 0;
                //all values from a specific gesture
                float[] valueList = new float[bufferSize];
                //loop trough all gestures in a discrete dictionairy
                foreach (string s in discreteResultsBuffer[0].Keys)
                {
                    //loop trough all dictionairies in the buffer and get the gesture values
                    for (int i = 0; i < bufferSize; i++)
                    {
                        discreteResultsBuffer[i].TryGetValue(s, out valueList[i]);
                    }
                    //add all floats up to a single value
                    for (int i = 0; i < bufferSize; i++)
                    {
                        finalValue = finalValue + valueList[i];
                    }
                    //get the average
                    finalValue = finalValue / bufferSize;
                    //add the final value to the returnable dictionairy
                    returnDict.Add(s, finalValue);
                    //reset the final value
                    finalValue = 0;
                }
            }
            else
            {
                return discreteResultsBuffer[0];
            }
            return returnDict;
        }

        /// <summary>
        /// creates a new dictionairy that averages the discrete results in the buffer
        /// </summary>
        /// <returns>a dictionairy filled with averege gesture results</returns>
        private IReadOnlyDictionary<string, float> getAverageContinuousResults()
        {
            //final result dictionairy
            Dictionary<string, float> returnDict = new Dictionary<string, float>();
            if(continuousResultsBuffer.Count() > 1)
            {
                //the final value after the average has been calculated
                float finalValue = 0;
                //all values from a specific gesture
                float[] valueList = new float[bufferSize];
                //loop trough all gestures in a continuous dictionairy
                foreach (string s in continuousResultsBuffer[0].Keys)
                {
                    //loop trough all dictionairies in the buffer and get the gesture values
                    for (int i = 0; i < bufferSize; i++)
                    {
                        continuousResultsBuffer[i].TryGetValue(s, out valueList[i]);
                    }
                    //add all floats up to a single value
                    for (int i = 0; i < bufferSize; i++)
                    {
                        finalValue = finalValue + valueList[i];
                    }
                    //get the average
                    finalValue = finalValue / bufferSize;
                    //add the final value to the returnable dictionairy
                    returnDict.Add(s, finalValue);
                    //reset the final value
                    finalValue = 0;
                }
            }
            else
            {
                return continuousResultsBuffer[0];
            }
                
            
            return returnDict;
        }


    }
}
