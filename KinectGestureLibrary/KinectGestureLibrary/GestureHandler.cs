using Microsoft.Kinect.VisualGestureBuilder;
using System;
using System.Collections.Generic;

namespace KinectGestureLibrary
{
    /// <summary>
    /// This class creates lists of PseudoGestures that represent a Microsoft.Kinect.VisualGestureBuilder.Gesture.
    /// The PseudoGestures can be used as normal gestures and represents a set of linked gestures.
    /// An example usage of an linked gesture is 2Pointing_LeftHanded" and "Pointing_RightHanded", these two can be
    /// linked to only trigger a single gesture "Pointing". The confidence/progress of these links always equals the 
    /// confidence/progress of the gesture with highest confidence/progress that is present in the link.
    /// 
    /// These links are defined in a configuration file that can be given to this gesturehandler.
    /// 
    /// TODO: 
    /// -implement a way to trigger events only when their gestures have a confidence/progress level above a defined treshold
    ///  (defined in the configuration file)
    /// -implement a way to only trigger certain events when gestures are completed in a specific order 
    ///  (Also known as Combined gestures, for instance you can have the program trigger on a discrete gesture making the
    ///   program log progress percentage of a secondary continuous gesture, 
    ///   then if the progress percentage reaches a hundred then the gesture was correctly performed)
    /// </summary>
    class GestureHandler
    {
        /// <summary> Path to the gesture database that was trained with VGB </summary>
        public string DatabaseLocation { get; set; }

        /// <summary>config that has been read from a file</summary>
        private GestureLibraryConfiguration conf;

        /// <summary>Eventhandler to send notifications</summary>
        private LibEventHandler evHandler { get; }

        /// <summary>
        /// Constructor for the GestureHandler class
        /// </summary>
        /// <param name="aDbLocation">location of the gesture database + name of the database</param>
        public GestureHandler(string aDbLocation, LibEventHandler aEvHandler)
        {
            this.DatabaseLocation = @aDbLocation;
            this.evHandler = aEvHandler;
        }

        /// <summary>
        /// Constructor for the GestureHandler class
        /// </summary>
        /// <param name="aDbLocation">location of the gesture database + name of the database</param>
        /// <param name="aConfLocation">location of a config file</param>
        public GestureHandler(string aDbLocation, LibEventHandler aEvHandler, GestureLibraryConfiguration aLibraryConf)
        {
            this.DatabaseLocation = @aDbLocation;
            this.evHandler = aEvHandler;
            this.conf = aLibraryConf;
        }

        /// <summary>
        /// Converts the default confidence result list to a list where gestures have been linked.
        /// </summary>
        /// <param name="aResultList">A list of unlinked gesture results</param>
        /// <returns>A dictionairy that uses PseudoGesture names to search for their confidence values</returns>
        public Dictionary<string, float> getLinkedDiscreteResults(IReadOnlyDictionary<Gesture, DiscreteGestureResult> aResultList)
        {
            Dictionary<string, float> returnDict = null;
            if (aResultList != null)
            {
                returnDict = new Dictionary<string, float>();
                GestureLink tempLink = null;
                bool ignored = false;
                //check every gesture in the resultList
                foreach (KeyValuePair<Gesture, DiscreteGestureResult> entry in aResultList)
                {
                    ignored = false;
                    tempLink = null;
                    if (conf != null)
                    {
                        //check if the current gesture should be ignored
                        foreach (string s in conf.ignoreList)
                        {
                            if (s.Equals(entry.Key.Name))
                            {
                                ignored = true;
                                break;
                            }
                        }

                        //check if the current gesture from VGBsource is included in a link from the config
                        if (!ignored)
                            tempLink = conf.findGestureInLink(entry.Key.Name);//null if not included
                    }

                    //was it included?
                    if (tempLink != null)
                    {
                        //if the dictionairy doesn't contain a gesture that represents a link, add a gesture to work as the link
                        if (!returnDict.ContainsKey(tempLink.commonName))
                        {
                            //In case of a link that is available, we only want to know the confidence of the gesture with highest confidence
                            //so in order to do that, we need to find out which of the gestures in a link have the highest confidence
                            DiscreteGestureResult gestResult = null;
                            float highestConfidence = 0;
                            //read all gesturenames from the linked gestures list
                            foreach (string gestureName in tempLink.linkedGesturesList)
                            {
                                //search trough the result dictionairy to find the correct gesture confidence
                                foreach (KeyValuePair<Gesture, DiscreteGestureResult> gesture in aResultList)
                                {
                                    //find the gesture that has the same name as the current gesture we got from the link list
                                    if (gesture.Key.Name.Equals(gestureName))
                                    {
                                        //read out the confidence this gesture has
                                        aResultList.TryGetValue(gesture.Key, out gestResult);
                                        //check if this gesture has the highest confidence
                                        if (highestConfidence < gestResult.Confidence)
                                        {
                                            //if so set this confidence as the final confidence
                                            highestConfidence = gestResult.Confidence;
                                        }
                                        //we found the gesture we wanted, no need to search trough the dictionary anymore
                                        break;
                                    }
                                }
                            }
                            returnDict.Add(tempLink.commonName, highestConfidence);
                        }
                    }// END OF --- templink != null
                    else
                    {
                        if (!ignored)
                        {
                            //if the gesture wasn't included in a link, we can add it directly to the returnDict
                            DiscreteGestureResult gestResult = null;
                            aResultList.TryGetValue(entry.Key, out gestResult);
                            returnDict.Add(entry.Key.Name, gestResult.Confidence);
                        }
                    }
                }
            }
            return returnDict;
        }

        /// <summary>
        /// Converts the default continuous result list to a list where gestures have been linked.
        /// </summary>
        /// <param name="aResultList">A list of unlinked gesture results</param>
        /// <returns>A dictionairy that uses PseudoGesture names to search for their continuous values</returns>
        public Dictionary<string, float> getLinkedContinuousResult(IReadOnlyDictionary<Gesture, ContinuousGestureResult> aResultList)
        {
            Dictionary<string, float> returnDict = null;
            if (aResultList != null)
            {
                returnDict = new Dictionary<string, float>();
                GestureLink tempLink = null;
                bool ignored = false;
                foreach (KeyValuePair<Gesture, ContinuousGestureResult> contResult in aResultList)
                {
                    ignored = false;
                    tempLink = null;
                    if (conf != null)
                    {
                        //check if the current gesture should be ignored
                        foreach (string s in conf.ignoreList)
                        {
                            if (s.Equals(contResult.Key.Name))
                            {
                                ignored = true;
                                break;
                            }
                        }
                        //check if the current gesture from VGBsource is included in a link from the config
                        if (!ignored)
                            tempLink = conf.findGestureInLink(contResult.Key.Name);//null if not included
                    }

                    //was it included?
                    if (tempLink != null)
                    {
                        //if the dictionairy doesn't contain a gesture that represents a link, add a gesture to work as the link
                        if (!returnDict.ContainsKey(tempLink.commonName))
                        {
                            //In case of a link that is available, we only want to know the confidence of the gesture with highest confidence
                            //so in order to do that, we need to find out which of the gestures in a link have the highest confidence
                            ContinuousGestureResult gestResult = null;
                            float highestProgress = 0;
                            //read all gesturenames from the linked gestures list
                            foreach (string gestureName in tempLink.linkedGesturesList)
                            {
                                //search trough the result dictionairy to find the correct gesture confidence
                                foreach (KeyValuePair<Gesture, ContinuousGestureResult> gesture in aResultList)
                                {
                                    //find the gesture that has the same name as the current gesture we got from the link list
                                    if (gesture.Key.Name.Equals(gestureName))
                                    {
                                        //read out the confidence this gesture has
                                        aResultList.TryGetValue(gesture.Key, out gestResult);
                                        //check if this gesture has the highest confidence
                                        if (highestProgress < gestResult.Progress)
                                        {
                                            //if so set this confidence as the final confidence
                                            highestProgress = gestResult.Progress;
                                        }
                                        //we found the gesture we wanted, no need to search trough the dictionary anymore
                                        break;
                                    }
                                }
                                
                            }
                            returnDict.Add(tempLink.commonName, highestProgress);
                        }
                    }
                    else
                    {
                        if (!ignored)
                        {
                            //if the gesture wasn't included in a link, we can add it directly to the returnDict
                            ContinuousGestureResult gestResult = null;
                            aResultList.TryGetValue(contResult.Key, out gestResult);
                            returnDict.Add(contResult.Key.Name, gestResult.Progress);
                        }
                    }
                }
            }
            return returnDict;
        }

        /// <summary>
        /// Reads from the configuration file and returns a list of gestures that are linked together
        /// </summary>
        /// <param name="aVgbSource">A VGB frame source which stores references to gestures in a database</param>
        /// <returns>A list of PseudoGestures that represent Linked Gestures</returns>
        public List<PseudoGesture> linkGestures(VisualGestureBuilderFrameSource aVgbSource)
        {
            List<PseudoGesture> returnList = new List<PseudoGesture>();
            GestureLink tempLink = null;
            bool ignored = false;
            foreach (Gesture gest in aVgbSource.Gestures)
            {
                ignored = false;
                tempLink = null;
                if (conf != null)
                {
                    //check if the current gesture should be ignored
                    foreach (string s in conf.ignoreList)
                    {
                        if (s.Equals(gest.Name))
                        {
                            ignored = true;
                            break;
                        }
                    }
                    
                    //check if the current gesture from VGBsource is included in a link from the config
                    if(!ignored)
                        tempLink = conf.findGestureInLink(gest.Name);//null if not included
                }
                //was it included?
                if (tempLink != null)
                {
                    string gestureType = "Discrete";
                    if(gest.GestureType == GestureType.Continuous)
                    {
                        gestureType = "Continuous";
                    }
                    if(tempLink.gestureType != null)
                    {
                        //Make sure that the discrete gestures get added to discrete links, and continuous with continuous
                        if (gestureType.Equals(tempLink.gestureType, StringComparison.OrdinalIgnoreCase))
                        {
                            //check if any elements are added to the returnList incase of when the current link was already added
                            if (returnList.Count > 0)
                            {
                                //if the list doesn't contain a gesture that represents a link, add a gesture to work as the link
                                if (!returnList.Exists(x => x.gestureName.Equals(tempLink.commonName)))
                                {
                                    returnList.Add(new PseudoGesture(tempLink.commonName, gest.GestureType));
                                }
                            }
                            else
                            {
                                //if the returnList is empty, we don't have to check if the link already has been added as gesture
                                returnList.Add(new PseudoGesture(tempLink.commonName, gest.GestureType));
                            }
                        }
                        else
                        {
                            System.Console.WriteLine("Configuration error, a discrete gesture was added to a continuous link or vice versa.");
                            evHandler.raiseMessageEvent(1, "Configuration error, a discrete gesture was added to a continuous link or vice versa.");
                        }
                    }else
                    {
                        Console.WriteLine("Configuration error, a link didn't have a defined gestureType.");
                        evHandler.raiseMessageEvent(1, "Configuration error, a link didn't have a defined gestureType.");
                    }
                    
                }else
                {
                    //if the gesture wasn't included in a link, we can add it directly to the returnlist
                    if(!ignored)
                        returnList.Add(new PseudoGesture(gest.Name, gest.GestureType));
                }
            }
            return returnList;
        }

        /// <summary>
        /// Returns the buffer size for the average filter.
        /// </summary>
        /// <returns>The buffer size to use</returns>
        public int getBufferSize()
        {
            return conf.AverageFilterBufferSize;
        }

    }
}
