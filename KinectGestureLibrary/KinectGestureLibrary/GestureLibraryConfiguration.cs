using System.Collections.Generic;
using System.Xml.Serialization;

namespace KinectGestureLibrary
{
    /// <summary>
    /// GestureLibraryConfiguration represents any settings that can be serialized and stored in a configuration file
    /// </summary>
    [XmlRoot(ElementName = "Config")]
    public class GestureLibraryConfiguration
    {
        /// <summary>Buffer size for the average filter used by the gesture detector</summary>
        public int AverageFilterBufferSize { get; set; }

        /// <summary> A list of linked gestures </summary>
        [XmlArray("LinkedGestures")]
        [XmlArrayItem("Link")]
        public List<GestureLink> linkList { get; }

        /// <summary> A list of ignored gestures </summary>
        [XmlArray("Ignored")]
        [XmlArrayItem("GestureName")]
        public List<string> ignoreList { get; }

        /// <summary>
        /// Constructor for the configuration
        /// </summary>
        public GestureLibraryConfiguration()
        {
            linkList = new List<GestureLink>();
            ignoreList = new List<string>();
        }

        /// <summary>
        /// Add a gestureName to the ignore list
        /// </summary>
        /// <param name="gestureNames">Names of the gestures that will be linked</param>
        /// <param name="linkName">The common name the link will have</param>
        public void addIgnore(string aIgnoreName)
        {
            ignoreList.Add(aIgnoreName);
        }


        /// <summary>
        /// Add a link to the link list
        /// </summary>
        /// <param name="aGestureNames">Names of the gestures that will be linked</param>
        /// <param name="aLinkName">The common name the link will have</param>
        /// <param name="aGestureType">The type of gesture the link should represent</param>
        public void addLink(string[] aGestureNames, string aLinkName, string aGestureType)
        {
            linkList.Add(new GestureLink(aGestureNames, aLinkName,aGestureType));
        }

        /// <summary>
        /// Get a specific link by checking if a gesture has been included in any link
        /// </summary>
        /// <param name="aGestureName">The gesture's name which will be used to find the link</param>
        /// <returns>A GestureLink</returns>
        public GestureLink findGestureInLink(string aGestureName)
        {
            foreach(GestureLink link in linkList)
            {
                if (link.findLinkedGesture(aGestureName))
                {
                    return link;
                }
            }
            return null;
        }
    }
}
