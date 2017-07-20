using System;
using System.Xml.Serialization;

namespace KinectGestureLibrary
{
    /// <summary>
    /// GestureLink represents a Link between gestures. Any gestures that have been linked
    /// will trigger the same event under the CommonName of this instance.
    /// </summary>
    public class GestureLink
    {
        /// <summary>The common name for the link</summary>
        public string CommonName;

        /// <summary>The type of gesture this link should represent</summary>
        public string GestureType;

        /// <summary>The list of gestures that should be linked</summary>
        [XmlArray("GestureNames")]
        public string[] linkedGestures;

        /// <summary>Get/set methods for above variables</summary>
        public string gestureType { get { return GestureType; } }
        public string commonName { get { return CommonName; } }
        public string[] linkedGesturesList { get { return linkedGestures; } }

        /// <summary>
        /// private constructor to make this object serializable
        /// </summary>
        private GestureLink() { }

        /// <summary>
        /// public constructor to create objects of type GestureLink
        /// </summary>
        /// <param name="aGestureNames">name-array of the gestures to link</param>
        /// <param name="aLinkName">name of the newly created link</param>
        /// <param name="aGestureType">The type of gesture the link should represent</param>
        public GestureLink(string[] aGestureNames, string aLinkName, string aGestureType)
        {
            this.linkedGestures = aGestureNames;
            this.CommonName = aLinkName;
            this.GestureType = aGestureType;
        }

        /// <summary>
        /// Finds if a gesture has been included in the link
        /// </summary>
        /// <param name="gestureName">name of the gesture that we want to know has been linked</param>
        /// <returns>returns true if the gesture is included in this link</returns>
        public bool findLinkedGesture(string aGestureName)
        {
            foreach(string s in linkedGestures)
            {
                if (s.Equals(aGestureName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
